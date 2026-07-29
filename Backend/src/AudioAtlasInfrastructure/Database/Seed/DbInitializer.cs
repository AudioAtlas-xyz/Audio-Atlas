using AudioAtlasDomain.Genres;
using AudioAtlasDomain.Geography;
using AudioAtlasDomain.MusicMetadata;
using AudioAtlasDomain.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AudioAtlasInfrastructure.Database.Seed;

public class DbInitializer
{
    private const string AdminRoleName = "Admin";
    private static string[] roles = [AdminRoleName, "Banned", "Curator"];
    private static string SystemUserName = "System";
    private static string DeletedUserName = "DeletedUser";
    public static async Task SeedDatabase(AppDbContext dbContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager, IConfiguration configuration, ILogger<DbInitializer> logger)
    {
        logger.LogInformation("Creating System User with username: {SystemUsername}", SystemUserName);

        ApplicationUser systemUser = await CreateStaticUserAsync(SystemUserName, dbContext, userManager, logger);

        logger.LogInformation("Creating System User with username: {DeletedUserName}", DeletedUserName);

        ApplicationUser DeletedUser = await CreateStaticUserAsync(DeletedUserName, dbContext, userManager, logger, isDeletedPlaceholder: true);

        logger.LogInformation("Creating Roles: {roles}", "[" + string.Join(", ", roles) + "]");

        await CreateRolesAsync(roles, roleManager, logger);

        await SeedAdminsAsync(configuration, userManager, logger);

        if (dbContext.Instruments.Any() || dbContext.Genres.Any() || dbContext.Countries.Any())
        {
            logger.LogWarning("Skipping initial seeding as database already contains data.");
            return;
        }

        string countryPath = GetSeedFilePath("countrySeeding.json");
        string instrumentPath = GetSeedFilePath("instrumentSeeding.json");
        string genrePath = GetSeedFilePath("genreSeeding.json");

        logger.LogInformation("Loading seed data from {SeedPath}", genrePath);

        string countryJson = File.ReadAllText(countryPath);
        string instrumentJson = File.ReadAllText(instrumentPath);
        string genreJson = File.ReadAllText(genrePath);

        Dictionary<string, Country> countryMapping;
        Dictionary<string, Instrument> instrumentMapping;

        using (JsonDocument doc = JsonDocument.Parse(countryJson))
        {
            countryMapping = ProcessCountries(doc.RootElement, logger);
            dbContext.Countries.AddRange(countryMapping.Values);
        }

        using (JsonDocument doc = JsonDocument.Parse(instrumentJson))
        {
            instrumentMapping = ProcessInstruments(doc.RootElement, logger);
            dbContext.Instruments.AddRange(instrumentMapping.Values);
        }

        using (JsonDocument doc = JsonDocument.Parse(genreJson))
        {
            JsonElement root = doc.RootElement;

            Dictionary<string, Genre> genreMapping = ProcessGenres(root.GetProperty("genres"), systemUser, logger);
            dbContext.Genres.AddRange(genreMapping.Values);

            ProcessGenreCountries(root, genreMapping, countryMapping, logger);
            ProcessGenreInstruments(root, genreMapping, instrumentMapping, logger);
            ProcessGenreHierarchy(root, genreMapping, logger);
            ProcessGenreAliases(root, genreMapping, logger);
            ProcessGenreSources(root, genreMapping, logger);

            dbContext.SaveChanges();
        }

    }

    internal static async Task SeedAdditionalDataAsync(AppDbContext dbContext, ILogger<DbInitializer> logger)
    {
        ApplicationUser? systemUser = await dbContext.Users.FirstOrDefaultAsync(u => u.IsSystemUser);
        if (systemUser == null)
        {
            logger.LogWarning("Supplemental seeder: system user not found, skipping.");
            return;
        }

        string? instrument2Path = null;
        string? genre2Path = null;

        try { instrument2Path = GetSeedFilePath("instrumentSeeding2.json"); } catch (FileNotFoundException) { }
        try { genre2Path = GetSeedFilePath("genreSeeding2.json"); } catch (FileNotFoundException) { }

        if (instrument2Path == null && genre2Path == null)
        {
            logger.LogInformation("No supplemental seed files found; skipping additional seeding.");
            return;
        }

        logger.LogInformation("Running supplemental seeder...");

        // ── New instruments ─────────────────────────────────────────────────────
        var instMapping = new Dictionary<string, Instrument>(StringComparer.OrdinalIgnoreCase);

        if (instrument2Path != null)
        {
            using JsonDocument instDoc = JsonDocument.Parse(File.ReadAllText(instrument2Path));
            foreach (JsonElement obj in instDoc.RootElement.EnumerateArray())
            {
                string? seedId = obj.GetProperty("Id").GetString();
                string? name = obj.GetProperty("Name").GetString();
                if (string.IsNullOrWhiteSpace(seedId) || string.IsNullOrWhiteSpace(name)) continue;

                Instrument? inst = dbContext.Instruments.Local.FirstOrDefault(i => i.Type == name)
                    ?? await dbContext.Instruments.FirstOrDefaultAsync(i => i.Type == name);
                if (inst == null)
                {
                    inst = new Instrument { Type = name, Description = TryGetStringProperty(obj, "Description") };
                    dbContext.Instruments.Add(inst);
                }
                instMapping[seedId] = inst;
            }
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Supplemental instruments: {Count} mapped", instMapping.Count);
        }

        if (genre2Path == null) return;

        using JsonDocument genreDoc = JsonDocument.Parse(File.ReadAllText(genre2Path));
        JsonElement root = genreDoc.RootElement;

        // Resolve existingInstrumentRefs into instMapping
        if (root.TryGetProperty("existingInstrumentRefs", out JsonElement existingInstRefs))
        {
            foreach (JsonProperty prop in existingInstRefs.EnumerateObject())
            {
                if (instMapping.ContainsKey(prop.Name)) continue;
                string instName = prop.Value.GetString()!;
                Instrument? inst = dbContext.Instruments.Local.FirstOrDefault(i => i.Type == instName)
                    ?? await dbContext.Instruments.FirstOrDefaultAsync(i => i.Type == instName);
                if (inst != null) instMapping[prop.Name] = inst;
            }
        }

        // ── New genres ──────────────────────────────────────────────────────────
        var genreMapping = new Dictionary<string, Genre>(StringComparer.OrdinalIgnoreCase);

        foreach (JsonElement obj in root.GetProperty("genres").EnumerateArray())
        {
            string? seedId = TryGetStringProperty(obj, "id");
            string? name = TryGetStringProperty(obj, "name");
            if (string.IsNullOrWhiteSpace(seedId) || string.IsNullOrWhiteSpace(name)) continue;

            Genre? genre = dbContext.Genres.Local.FirstOrDefault(g => g.Name == name)
                ?? await dbContext.Genres.FirstOrDefaultAsync(g => g.Name == name);
            if (genre == null)
            {
                genre = new Genre
                {
                    Name = name,
                    Description = TryGetStringProperty(obj, "description"),
                    StartYear = ParseStartYear(obj),
                    IsSensitive = TryGetBoolProperty(obj, "isSensitive"),
                    SensitiveDescription = TryGetStringProperty(obj, "sensitiveDescription"),
                    ExampleSongYoutubeId = TryGetStringProperty(obj, "exampleSongYoutubeId"),
                    AuthorId = systemUser.Id
                };
                dbContext.Genres.Add(genre);
            }
            genreMapping[seedId] = genre;
        }
        await dbContext.SaveChangesAsync();
        logger.LogInformation("Supplemental genres: {Count} mapped", genreMapping.Count);

        // Resolve existingGenreRefs into genreMapping
        if (root.TryGetProperty("existingGenreRefs", out JsonElement existingGenreRefs))
        {
            foreach (JsonProperty prop in existingGenreRefs.EnumerateObject())
            {
                if (genreMapping.ContainsKey(prop.Name)) continue;
                string genreName = prop.Value.GetString()!;
                Genre? genre = dbContext.Genres.Local.FirstOrDefault(g => g.Name == genreName)
                    ?? await dbContext.Genres.FirstOrDefaultAsync(g => g.Name == genreName);
                if (genre != null) genreMapping[prop.Name] = genre;
            }
        }

        // ── Countries lookup ────────────────────────────────────────────────────
        // Built defensively rather than with ToDictionaryAsync. isoCode has no
        // uniqueness constraint, and a single duplicated row used to throw here —
        // which aborted the whole supplemental seed *after* genres had been saved
        // but *before* any countries, instruments or relations were linked,
        // leaving the catalogue full of orphaned genres. A duplicate should
        // degrade to a warning, not silently destroy every relation.
        var countryLookup = new Dictionary<string, Country>();
        foreach (Country country in await dbContext.Countries.ToListAsync())
        {
            if (!countryLookup.TryAdd(country.isoCode, country))
            {
                logger.LogWarning(
                    "Duplicate country isoCode '{IsoCode}' in the Countries table; keeping the first row and ignoring the duplicate. Clean this up in the database.",
                    country.isoCode);
            }
        }

        // ── Helper: get Guid set of genres involved in a section ────────────────
        static HashSet<Guid> CollectGenreIds(JsonElement arr, string key, Dictionary<string, Genre> map)
        {
            var ids = new HashSet<Guid>();
            foreach (JsonElement obj in arr.EnumerateArray())
            {
                string? sid = TryGetStringPropertyStatic(obj, key);
                if (sid != null && map.TryGetValue(sid, out Genre? g)) ids.Add(g.Id);
            }
            return ids;
        }

        // ── genreCountry ────────────────────────────────────────────────────────
        if (root.TryGetProperty("genreCountry", out JsonElement gcArr) && gcArr.ValueKind == JsonValueKind.Array)
        {
            var ids = CollectGenreIds(gcArr, "genreId", genreMapping);
            var loaded = await dbContext.Genres.Include(g => g.Countries)
                .Where(g => ids.Contains(g.Id)).ToDictionaryAsync(g => g.Id);
            int n = 0;
            foreach (JsonElement obj in gcArr.EnumerateArray())
            {
                if (!genreMapping.TryGetValue(TryGetStringProperty(obj, "genreId") ?? "", out Genre? gRef)) continue;
                if (!loaded.TryGetValue(gRef.Id, out Genre? genre)) continue;
                if (!countryLookup.TryGetValue(TryGetStringProperty(obj, "countryId") ?? "", out Country? country)) continue;
                if (!genre.Countries.Any(c => c.Id == country.Id)) { genre.Countries.Add(country); n++; }
            }
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Supplemental genreCountry: {Count} added", n);
        }

        // ── genreInstrument ─────────────────────────────────────────────────────
        if (root.TryGetProperty("genreInstrument", out JsonElement giArr) && giArr.ValueKind == JsonValueKind.Array)
        {
            var ids = CollectGenreIds(giArr, "genreId", genreMapping);
            var loaded = await dbContext.Genres.Include(g => g.Instruments)
                .Where(g => ids.Contains(g.Id)).ToDictionaryAsync(g => g.Id);
            int n = 0;
            foreach (JsonElement obj in giArr.EnumerateArray())
            {
                if (!genreMapping.TryGetValue(TryGetStringProperty(obj, "genreId") ?? "", out Genre? gRef)) continue;
                if (!loaded.TryGetValue(gRef.Id, out Genre? genre)) continue;
                if (!instMapping.TryGetValue(TryGetStringProperty(obj, "instrumentId") ?? "", out Instrument? inst)) continue;
                if (!genre.Instruments.Any(i => i.Id == inst.Id)) { genre.Instruments.Add(inst); n++; }
            }
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Supplemental genreInstrument: {Count} added", n);
        }

        // ── genreHierarchy ──────────────────────────────────────────────────────
        if (root.TryGetProperty("genreHierarchy", out JsonElement ghArr) && ghArr.ValueKind == JsonValueKind.Array)
        {
            var ids = new HashSet<Guid>();
            foreach (JsonElement obj in ghArr.EnumerateArray())
            {
                foreach (string key in new[] { "genreId", "subgenreId" })
                {
                    if (genreMapping.TryGetValue(TryGetStringProperty(obj, key) ?? "", out Genre? g)) ids.Add(g.Id);
                }
            }
            var loaded = await dbContext.Genres.Include(g => g.SubGenres)
                .Where(g => ids.Contains(g.Id)).ToDictionaryAsync(g => g.Id);
            int n = 0;
            foreach (JsonElement obj in ghArr.EnumerateArray())
            {
                if (!genreMapping.TryGetValue(TryGetStringProperty(obj, "genreId") ?? "", out Genre? pRef)) continue;
                if (!genreMapping.TryGetValue(TryGetStringProperty(obj, "subgenreId") ?? "", out Genre? cRef)) continue;
                if (!loaded.TryGetValue(pRef.Id, out Genre? parent)) continue;
                if (!loaded.TryGetValue(cRef.Id, out Genre? child)) continue;
                if (!parent.SubGenres.Any(s => s.Id == child.Id)) { parent.SubGenres.Add(child); n++; }
            }
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Supplemental genreHierarchy: {Count} added", n);
        }

        // ── genreSimilarity ─────────────────────────────────────────────────────
        if (root.TryGetProperty("genreSimilarity", out JsonElement gsArr) && gsArr.ValueKind == JsonValueKind.Array)
        {
            var ids = new HashSet<Guid>();
            foreach (JsonElement obj in gsArr.EnumerateArray())
            {
                foreach (string key in new[] { "similarId1", "similarId2" })
                {
                    if (genreMapping.TryGetValue(TryGetStringProperty(obj, key) ?? "", out Genre? g)) ids.Add(g.Id);
                }
            }
            var loaded = await dbContext.Genres.Include(g => g.SimilarGenres)
                .Where(g => ids.Contains(g.Id)).ToDictionaryAsync(g => g.Id);
            int n = 0;
            foreach (JsonElement obj in gsArr.EnumerateArray())
            {
                if (!genreMapping.TryGetValue(TryGetStringProperty(obj, "similarId1") ?? "", out Genre? r1)) continue;
                if (!genreMapping.TryGetValue(TryGetStringProperty(obj, "similarId2") ?? "", out Genre? r2)) continue;
                if (!loaded.TryGetValue(r1.Id, out Genre? g1)) continue;
                if (!loaded.TryGetValue(r2.Id, out Genre? g2)) continue;
                if (!g1.SimilarGenres.Any(s => s.Id == g2.Id)) { g1.SimilarGenres.Add(g2); n++; }
                if (!g2.SimilarGenres.Any(s => s.Id == g1.Id)) { g2.SimilarGenres.Add(g1); n++; }
            }
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Supplemental genreSimilarity: {Count} added", n);
        }

        // ── genreAliases ────────────────────────────────────────────────────────
        if (root.TryGetProperty("genreAliases", out JsonElement gaArr) && gaArr.ValueKind == JsonValueKind.Array)
        {
            var ids = CollectGenreIds(gaArr, "genreId", genreMapping);
            var loaded = await dbContext.Genres.Include(g => g.Aliases)
                .Where(g => ids.Contains(g.Id)).ToDictionaryAsync(g => g.Id);
            int n = 0;
            foreach (JsonElement obj in gaArr.EnumerateArray())
            {
                if (!genreMapping.TryGetValue(TryGetStringProperty(obj, "genreId") ?? "", out Genre? gRef)) continue;
                if (!loaded.TryGetValue(gRef.Id, out Genre? genre)) continue;
                string? alias = TryGetStringProperty(obj, "alias");
                if (string.IsNullOrWhiteSpace(alias)) continue;
                if (!genre.Aliases.Any(a => string.Equals(a.Alias, alias, StringComparison.OrdinalIgnoreCase)))
                {
                    genre.Aliases.Add(new GenreAlias { Genre = genre, GenreId = genre.Id, Alias = alias });
                    n++;
                }
            }
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Supplemental genreAliases: {Count} added", n);
        }

        // ── genreSources ────────────────────────────────────────────────────────
        if (root.TryGetProperty("genreSources", out JsonElement gsSrcArr) && gsSrcArr.ValueKind == JsonValueKind.Array)
        {
            var ids = CollectGenreIds(gsSrcArr, "genreId", genreMapping);
            var loaded = await dbContext.Genres.Include(g => g.Sources)
                .Where(g => ids.Contains(g.Id)).ToDictionaryAsync(g => g.Id);
            int n = 0;
            foreach (JsonElement obj in gsSrcArr.EnumerateArray())
            {
                if (!genreMapping.TryGetValue(TryGetStringProperty(obj, "genreId") ?? "", out Genre? gRef)) continue;
                if (!loaded.TryGetValue(gRef.Id, out Genre? genre)) continue;
                string? src = TryGetStringProperty(obj, "sourceLink");
                if (string.IsNullOrWhiteSpace(src)) continue;
                if (!genre.Sources.Any(s => string.Equals(s.SourceLink, src, StringComparison.OrdinalIgnoreCase)))
                {
                    genre.Sources.Add(new GenreSource { Genre = genre, GenreId = genre.Id, SourceLink = src });
                    n++;
                }
            }
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Supplemental genreSources: {Count} added", n);
        }

        await LogDataQualityAsync(dbContext, logger);

        logger.LogInformation("Supplemental seeding complete.");
    }

    /// <summary>
    /// Post-seed data-quality summary.
    ///
    /// The supplemental seeder once aborted part-way through for days without
    /// anyone noticing: the API stayed healthy, deploys stayed green, and the only
    /// symptom was a single error line in the container log. Genres were left with
    /// no country, which made them invisible on the globe.
    ///
    /// Emitting the counts on every startup gives that failure mode a stable,
    /// greppable signal to alert on, rather than relying on someone reading logs.
    /// </summary>
    internal static async Task LogDataQualityAsync(AppDbContext dbContext, ILogger<DbInitializer> logger)
    {
        int orphanGenres = await dbContext.Genres.CountAsync(g => !g.Countries.Any());
        int genreTotal = await dbContext.Genres.CountAsync();

        if (orphanGenres > 0)
        {
            logger.LogWarning(
                "DataQuality: {OrphanCount} of {GenreTotal} genres have no country link and will not appear on the globe.",
                orphanGenres, genreTotal);
        }
        else
        {
            logger.LogInformation("DataQuality: all {GenreTotal} genres have at least one country link.", genreTotal);
        }
    }

    private static string? TryGetStringPropertyStatic(JsonElement element, string propertyName)
        => TryGetStringProperty(element, propertyName);

    // Grants Admin to every email in Admin:SeedEmails. Runs on every
    // startup, skips users already in the role, warns (and retries next
    // startup) if a configured email has no matching user yet.
    private static async Task SeedAdminsAsync(
        IConfiguration configuration,
        UserManager<ApplicationUser> userManager,
        ILogger<DbInitializer> logger)
    {
        var seedEmails = configuration.GetSection("Admin:SeedEmails").Get<string[]>()
            ?? Array.Empty<string>();

        if (seedEmails.Length == 0)
        {
            logger.LogInformation("No admin seed emails configured; skipping admin seed.");
            return;
        }

        foreach (var email in seedEmails)
        {
            if (string.IsNullOrWhiteSpace(email))
                continue;

            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                logger.LogWarning(
                    "Admin seed: no user found with email {Email}. They will be granted Admin on the next startup after they first sign in.",
                    email);
                continue;
            }

            if (await userManager.IsInRoleAsync(user, AdminRoleName))
            {
                logger.LogInformation("Admin seed: {Email} already has the Admin role.", email);
                continue;
            }

            var result = await userManager.AddToRoleAsync(user, AdminRoleName);

            if (result.Succeeded)
            {
                logger.LogInformation("Admin seed: granted Admin role to {Email}.", email);
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    logger.LogError(
                        "Admin seed: failed to grant Admin role to {Email}: {Code} - {Description}",
                        email, error.Code, error.Description);
                }
            }
        }
    }

    private static async Task CreateRolesAsync(string[] roles, RoleManager<IdentityRole<Guid>> roleManager, ILogger<DbInitializer> logger)
    {
        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var result = await roleManager.CreateAsync(new IdentityRole<Guid>
                {
                    Name = roleName
                });

                foreach (var error in result.Errors)
                {
                    logger.LogError("Role error trying to create {RoleName}: {Code} - {Description}", roleName, error.Code, error.Description);
                }
            }
        }
    }

    private static async Task<ApplicationUser> CreateStaticUserAsync(string username, AppDbContext dbContext, UserManager<ApplicationUser> userManager, ILogger<DbInitializer> logger, bool isDeletedPlaceholder = false)
    {
        ApplicationUser? staticUser = await userManager.FindByNameAsync(username);

        if (staticUser == null)
        {
            staticUser = new ApplicationUser
            {
                UserName = username,
                Email = $"{username.ToLowerInvariant()}@audioatlas.com",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                LockoutEnabled = true,
                LockoutEnd = DateTimeOffset.MaxValue,
                IsSystemUser = !isDeletedPlaceholder,
                IsDeletedPlaceholder = isDeletedPlaceholder
            };

            // Create without password using a bypass
            var result = await userManager.CreateAsync(staticUser);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    logger.LogError("Identity error: {Code} - {Description}", error.Code, error.Description);
                }

                throw new Exception($"Could not create {username}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            await dbContext.SaveChangesAsync();
        }
        else if (!staticUser.IsSystemUser && !isDeletedPlaceholder)
        {
            staticUser.IsSystemUser = true;
            await dbContext.SaveChangesAsync();
        }
        else if (!staticUser.IsDeletedPlaceholder && isDeletedPlaceholder)
        {
            staticUser.IsDeletedPlaceholder = true;
            await dbContext.SaveChangesAsync();
        }


        return staticUser;  

    }

    private static string GetSeedFilePath(string fileName)
    {
        string[] candidates =
        {
            Path.Combine(AppContext.BaseDirectory, fileName),
            Path.Combine(AppContext.BaseDirectory, "resources", fileName),
            Path.Combine(Directory.GetCurrentDirectory(), "resources", fileName)
        };

        foreach (string path in candidates)
        {
            if (File.Exists(path))
            {
                return path;
            }
        }

        throw new FileNotFoundException($"Could not locate seed file '{fileName}'.");
    }

    private static Dictionary<string, Country> ProcessCountries(JsonElement countryRoot, ILogger<DbInitializer> logger)
    {
        Dictionary<string, Country> countryMapping = new();
        int countryCount = 0;

        foreach (JsonElement obj in countryRoot.EnumerateArray())
        {
            string? id = obj.GetProperty("Id").GetString();
            string? name = obj.GetProperty("Name").GetString();
            string? region = obj.GetProperty("Region").GetString();
            string? continent = obj.GetProperty("Continent").GetString();
            string? description = obj.GetProperty("Description").GetString();

            if (string.IsNullOrWhiteSpace(id))
            {
                logger.LogWarning("Skipping seed country with no id.");
                continue;
            }

            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(region) ||
                string.IsNullOrWhiteSpace(continent) ||
                string.IsNullOrWhiteSpace(description))
            {
                logger.LogWarning("Skipping seed country with source id {SourceId} because required fields are missing.", id);
                continue;
            }

            if (countryMapping.ContainsKey(id))
            {
                logger.LogWarning("Skipping seed country with source id {SourceId} because it is a duplicate.", id);
                continue;
            }

            Country country = new()
            {
                Name = name,
                Description = description,
                Region = region,
                Continent = continent,
                isoCode = id
            };

            countryMapping.Add(id, country);
            countryCount++;
        }

        logger.LogInformation("Processed {CountryCount} countries from seed data", countryCount);
        return countryMapping;
    }

    private static Dictionary<string, Instrument> ProcessInstruments(JsonElement instrumentRoot, ILogger<DbInitializer> logger)
    {
        Dictionary<string, Instrument> instrumentMapping = new();
        int instrumentCount = 0;

        foreach (JsonElement obj in instrumentRoot.EnumerateArray())
        {
            string? id = obj.GetProperty("Id").GetString();
            string? name = obj.GetProperty("Name").GetString();
            string? type = obj.GetProperty("Type").GetString();
            string? description = obj.GetProperty("Description").GetString();

            if (string.IsNullOrWhiteSpace(id))
            {
                logger.LogWarning("Skipping seed instrument with no id.");
                continue;
            }

            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(type) ||
                string.IsNullOrWhiteSpace(description))
            {
                logger.LogWarning("Skipping seed instrument with source id {SourceId} because required fields are missing.", id);
                continue;
            }

            if (instrumentMapping.ContainsKey(id))
            {
                logger.LogWarning("Skipping seed instrument with source id {SourceId} because it is a duplicate.", id);
                continue;
            }

            Instrument instrument = new()
            {
                Type = name,
                Description = description
            };

            instrumentMapping.Add(id, instrument);
            instrumentCount++;
        }

        logger.LogInformation("Processed {InstrumentCount} instruments from seed data", instrumentCount);
        return instrumentMapping;
    }

    private static Dictionary<string, Genre> ProcessGenres(JsonElement genreRoot, ApplicationUser systemUser, ILogger<DbInitializer> logger)
    {
        Dictionary<string, Genre> genreMapping = new();
        int genreCount = 0;

        foreach (JsonElement obj in genreRoot.EnumerateArray())
        {
            string? id = TryGetStringProperty(obj, "id");
            string? name = TryGetStringProperty(obj, "name");

            if (string.IsNullOrWhiteSpace(id))
            {
                logger.LogWarning("Skipping seed genre with no id.");
                continue;
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                logger.LogWarning("Skipping seed genre with source id {SourceId} because it has no name.", id);
                continue;
            }

            if (genreMapping.ContainsKey(id))
            {
                logger.LogWarning("Skipping seed genre with source id {SourceId} because it is a duplicate.", id);
                continue;
            }

            Genre genre = new()
            {
                Name = name,
                Description = TryGetStringProperty(obj, "description"),
                StartYear = ParseStartYear(obj),
                IsSensitive = TryGetBoolProperty(obj, "isSensitive"),
                SensitiveDescription = TryGetStringProperty(obj, "sensitiveDescription"),
                ExampleSongYoutubeId = TryGetStringProperty(obj, "exampleSongYoutubeId"),
                AuthorId = systemUser.Id
            };

            genreMapping.Add(id, genre);
            genreCount++;
        }

        logger.LogInformation("Processed {GenreCount} genres from seed data", genreCount);
        return genreMapping;
    }

    private static void ProcessGenreCountries(
        JsonElement root,
        Dictionary<string, Genre> genreMapping,
        Dictionary<string, Country> countryMapping,
        ILogger<DbInitializer> logger)
    {
        ProcessJoinTable(
            root,
            "genreCountry",
            "genreId",
            "countryId",
            genreMapping,
            countryMapping,
            (genre, country) =>
            {
                if (!genre.Countries.Any(x => x.Id == country.Id))
                {
                    genre.Countries.Add(country);
                }

                if (!country.Genres.Any(x => x.Id == genre.Id))
                {
                    country.Genres.Add(genre);
                }
            },
            logger);
    }

    private static void ProcessGenreInstruments(
        JsonElement root,
        Dictionary<string, Genre> genreMapping,
        Dictionary<string, Instrument> instrumentMapping,
        ILogger<DbInitializer> logger)
    {
        ProcessJoinTable(
            root,
            "genreInstrument",
            "genreId",
            "instrumentId",
            genreMapping,
            instrumentMapping,
            (genre, instrument) =>
            {
                if (!genre.Instruments.Any(x => x.Id == instrument.Id))
                {
                    genre.Instruments.Add(instrument);
                }

                if (!instrument.Genres.Any(x => x.Id == genre.Id))
                {
                    instrument.Genres.Add(genre);
                }
            },
            logger);
    }

    private static void ProcessGenreHierarchy(
        JsonElement root,
        Dictionary<string, Genre> genreMapping,
        ILogger<DbInitializer> logger)
    {
        int relationshipCount = 0;

        relationshipCount += ProcessGenreToGenreLinks(root, "genreHierarchy", "genreId", "subgenreId", genreMapping,
            (genre, related) =>
            {
                if (!genre.SubGenres.Any(x => x.Id == related.Id))
                {
                    genre.SubGenres.Add(related);
                    return true;
                }

                return false;
            },
            (genre, related) =>
            {
                if (!related.ParentGenres.Any(x => x.Id == genre.Id))
                {
                    related.ParentGenres.Add(genre);
                    return true;
                }

                return false;
            });

        relationshipCount += ProcessGenreToGenreLinks(root, "genreSimilarity", "similarId1", "similarId2", genreMapping,
            (genre, related) =>
            {
                if (!genre.SimilarGenres.Any(x => x.Id == related.Id))
                {
                    genre.SimilarGenres.Add(related);
                    return true;
                }

                return false;
            },
            (genre, related) =>
            {
                if (!related.SimilarGenres.Any(x => x.Id == genre.Id))
                {
                    related.SimilarGenres.Add(genre);
                    return true;
                }

                return false;
            });

        logger.LogInformation("Processed {RelationshipCount} genre relationships from seed data", relationshipCount);
    }

    private static void ProcessGenreAliases(JsonElement root, Dictionary<string, Genre> genreMapping, ILogger<DbInitializer> logger)
    {
        if (!root.TryGetProperty("genreAliases", out JsonElement genreAliases) || genreAliases.ValueKind != JsonValueKind.Array)
        {
            logger.LogInformation("No genre aliases found in seed data.");
            return;
        }

        int aliasCount = 0;

        foreach (JsonElement obj in genreAliases.EnumerateArray())
        {
            string? genreId = TryGetStringProperty(obj, "genreId");
            string? aliasValue = TryGetStringProperty(obj, "alias");

            if (string.IsNullOrWhiteSpace(genreId) ||
                string.IsNullOrWhiteSpace(aliasValue) ||
                !genreMapping.TryGetValue(genreId, out Genre? genre))
            {
                continue;
            }

            if (genre.Aliases.Any(x => string.Equals(x.Alias, aliasValue, StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            genre.Aliases.Add(new GenreAlias
            {
                Genre = genre,
                GenreId = genre.Id,
                Alias = aliasValue
            });

            aliasCount++;
        }

        logger.LogInformation("Processed {AliasCount} genre aliases from seed data", aliasCount);
    }

    private static void ProcessGenreSources(JsonElement root, Dictionary<string, Genre> genreMapping, ILogger<DbInitializer> logger)
    {
        if (!root.TryGetProperty("genreSources", out JsonElement genreSources) || genreSources.ValueKind != JsonValueKind.Array)
        {
            logger.LogInformation("No genre sources found in seed data.");
            return;
        }

        int sourceCount = 0;

        foreach (JsonElement obj in genreSources.EnumerateArray())
        {
            string? genreId = TryGetStringProperty(obj, "genreId");
            string? sourceLink = TryGetStringProperty(obj, "sourceLink");

            if (string.IsNullOrWhiteSpace(genreId) ||
                string.IsNullOrWhiteSpace(sourceLink) ||
                !genreMapping.TryGetValue(genreId, out Genre? genre))
            {
                continue;
            }

            if (genre.Sources.Any(x => string.Equals(x.SourceLink, sourceLink, StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            genre.Sources.Add(new GenreSource
            {
                Genre = genre,
                GenreId = genre.Id,
                SourceLink = sourceLink
            });

            sourceCount++;
        }

        logger.LogInformation("Processed {SourceCount} genre sources from seed data", sourceCount);
    }

    private static void ProcessJoinTable<TRelated>(
        JsonElement root,
        string tableName,
        string genrePropertyName,
        string relatedPropertyName,
        Dictionary<string, Genre> genreMapping,
        Dictionary<string, TRelated> relatedMapping,
        Action<Genre, TRelated> linkAction,
        ILogger<DbInitializer> logger)
    {
        if (!root.TryGetProperty(tableName, out JsonElement joinArray) || joinArray.ValueKind != JsonValueKind.Array)
        {
            logger.LogInformation("No {TableName} entries found in seed data.", tableName);
            return;
        }

        int relationshipCount = 0;

        foreach (JsonElement obj in joinArray.EnumerateArray())
        {
            string? genreId = TryGetStringProperty(obj, genrePropertyName);
            string? relatedId = TryGetStringProperty(obj, relatedPropertyName);

            if (string.IsNullOrWhiteSpace(genreId) ||
                string.IsNullOrWhiteSpace(relatedId) ||
                !genreMapping.TryGetValue(genreId, out Genre? genre) ||
                !relatedMapping.TryGetValue(relatedId, out TRelated? related))
            {
                continue;
            }

            linkAction(genre, related);
            relationshipCount++;
        }

        logger.LogInformation("Processed {RelationshipCount} {TableName} relationships from seed data", relationshipCount, tableName);
    }

    private static int ProcessGenreToGenreLinks(
        JsonElement root,
        string tableName,
        string leftPropertyName,
        string rightPropertyName,
        Dictionary<string, Genre> genreMapping,
        Func<Genre, Genre, bool> addLeftRelationship,
        Func<Genre, Genre, bool> addRightRelationship)
    {
        if (!root.TryGetProperty(tableName, out JsonElement joinArray) || joinArray.ValueKind != JsonValueKind.Array)
        {
            return 0;
        }

        int relationshipCount = 0;

        foreach (JsonElement obj in joinArray.EnumerateArray())
        {
            string? leftId = TryGetStringProperty(obj, leftPropertyName);
            string? rightId = TryGetStringProperty(obj, rightPropertyName);

            if (string.IsNullOrWhiteSpace(leftId) ||
                string.IsNullOrWhiteSpace(rightId) ||
                !genreMapping.TryGetValue(leftId, out Genre? leftGenre) ||
                !genreMapping.TryGetValue(rightId, out Genre? rightGenre))
            {
                continue;
            }

            if (addLeftRelationship(leftGenre, rightGenre))
            {
                relationshipCount++;
            }

            if (addRightRelationship(leftGenre, rightGenre))
            {
                relationshipCount++;
            }
        }

        return relationshipCount;
    }

    private static int? ParseStartYear(JsonElement element)
    {
        string? startDate = TryGetStringProperty(element, "startDate");

        if (string.IsNullOrWhiteSpace(startDate))
        {
            return null;
        }

        return DateTime.TryParse(startDate, out DateTime parsedDate)
            ? parsedDate.Year
            : null;
    }

    private static bool TryGetBoolProperty(JsonElement element, string propertyName)
    {
        return element.TryGetProperty(propertyName, out JsonElement property)
               && property.ValueKind is JsonValueKind.True or JsonValueKind.False
               && property.GetBoolean();
    }

    private static string? TryGetStringProperty(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out JsonElement property))
        {
            return null;
        }

        return property.ValueKind == JsonValueKind.Null ? null : property.GetString();
    }
}
