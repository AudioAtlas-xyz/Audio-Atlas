using System.Text.Json;
using AudioAtlasDomain.Users;
using AudioAtlasInfrastructure.Database;
using AudioAtlasInfrastructure.Database.Seed;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AudioAtlasInfrastructureTests;

public class DbInitializerTests
{
    [Fact]
    public async Task SeedDatabase_seeds_empty_database_with_expected_entities_and_system_user()
    {
        using var harness = new DbInitializerTestHarness();
        SeedCounts expected = ReadSeedCounts();

        await DbInitializer.SeedDatabase(harness.Context, harness.UserManager, harness.RoleManager, harness.Configuration, harness.Logger);

        ApplicationUser? systemUser = await harness.Context.Users.SingleOrDefaultAsync(u => u.UserName == "System");

        Assert.NotNull(systemUser);
        Assert.Equal(expected.CountryCount, await harness.Context.Countries.CountAsync());
        Assert.Equal(expected.InstrumentCount, await harness.Context.Instruments.CountAsync());
        Assert.Equal(expected.GenreCount, await harness.Context.Genres.CountAsync());
        Assert.All(await harness.Context.Genres.Select(g => g.AuthorId).ToListAsync(), authorId => Assert.Equal(systemUser!.Id, authorId));
    }

    [Fact]
    public async Task SeedDatabase_seeds_empty_database_with_expected_roles()
    {
        using var harness = new DbInitializerTestHarness();
        SeedCounts expected = ReadSeedCounts();

        await DbInitializer.SeedDatabase(harness.Context, harness.UserManager, harness.RoleManager, harness.Configuration, harness.Logger);

        string[] ExpectedRoleNames = ["Admin", "Banned", "Curator"];
        
        List<IdentityRole<Guid>> ActualRoles = await harness.Context.Roles.ToListAsync();

        Assert.NotNull(ActualRoles);
        Assert.True(ActualRoles.Any());
        Assert.Equal(expected.RoleCount, ActualRoles.Count);

        foreach (IdentityRole<Guid> role in ActualRoles)
        {
            Assert.True(ExpectedRoleNames.Contains(role.Name));
        }
    }


    [Fact]
    public async Task SeedDatabase_builds_expected_relationship_graph_from_seed_files()
    {
        using var harness = new DbInitializerTestHarness();
        SeedCounts expected = ReadSeedCounts();

        await DbInitializer.SeedDatabase(harness.Context, harness.UserManager, harness.RoleManager, harness.Configuration, harness.Logger);

        List<AudioAtlasDomain.Genres.Genre> genres = await harness.Context.Genres
            .Include(g => g.Author)
            .Include(g => g.Countries)
            .Include(g => g.Instruments)
            .Include(g => g.Aliases)
            .Include(g => g.Sources)
            .Include(g => g.SubGenres)
            .Include(g => g.ParentGenres)
            .Include(g => g.SimilarGenres)
            .ToListAsync();

        List<AudioAtlasDomain.Geography.Country> countries = await harness.Context.Countries
            .Include(c => c.Genres)
            .ToListAsync();

        List<AudioAtlasDomain.MusicMetadata.Instrument> instruments = await harness.Context.Instruments
            .Include(i => i.Genres)
            .ToListAsync();

        Assert.Equal(expected.AliasCount, genres.Sum(g => g.Aliases.Count));
        Assert.Equal(expected.SourceCount, genres.Sum(g => g.Sources.Count));
        Assert.Equal(expected.GenreCountryCount, countries.Sum(c => c.Genres.Count));
        Assert.Equal(expected.GenreInstrumentCount, instruments.Sum(i => i.Genres.Count));
        Assert.Equal(expected.GenreHierarchyCount, genres.Sum(g => g.SubGenres.Count));
        Assert.Equal(expected.GenreHierarchyCount, genres.Sum(g => g.ParentGenres.Count));
        Assert.Equal(expected.GenreSimilarityCount * 2, genres.Sum(g => g.SimilarGenres.Count));
        Assert.Contains(genres, g => g.Author is not null && g.Countries.Count > 0 && g.Instruments.Count > 0);
        Assert.Contains(genres, g => g.Aliases.Count > 0);
        Assert.Contains(genres, g => g.Sources.Count > 0);
    }

    [Fact]
    public async Task SeedDatabase_reuses_existing_system_user_as_genre_author()
    {
        using var harness = new DbInitializerTestHarness();

        ApplicationUser existingSystemUser = new()
        {
            UserName = "system",
            Email = "system@audioatlas.com",
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString(),
            LockoutEnabled = true,
            LockoutEnd = DateTimeOffset.MaxValue,
            IsSystemUser = true
        };

        IdentityResult createResult = await harness.UserManager.CreateAsync(existingSystemUser);
        Assert.True(createResult.Succeeded, string.Join(", ", createResult.Errors.Select(e => e.Description)));

        await DbInitializer.SeedDatabase(harness.Context, harness.UserManager, harness.RoleManager, harness.Configuration, harness.Logger);

        string? normalizedSystemUserName = harness.UserManager.NormalizeName("System");
        List<ApplicationUser> users = await harness.Context.Users
            .Where(u => u.NormalizedUserName == normalizedSystemUserName)
            .ToListAsync();

        Assert.Single(users);
        Assert.Equal(existingSystemUser.Id, users[0].Id);
        Assert.True(users[0].IsSystemUser);
        Assert.All(await harness.Context.Genres.Select(g => g.AuthorId).ToListAsync(), authorId => Assert.Equal(existingSystemUser.Id, authorId));
    }

    [Fact]
    public async Task SeedDatabase_skips_domain_seed_when_domain_data_already_exists()
    {
        using var harness = new DbInitializerTestHarness();

        harness.Context.Countries.Add(new AudioAtlasDomain.Geography.Country
        {
            Name = "Existing Country",
            Region = "Existing Region",
            Continent = "Existing Continent",
            Description = "Existing description",
            isoCode = "EXISTING"
        });
        await harness.Context.SaveChangesAsync();

        await DbInitializer.SeedDatabase(harness.Context, harness.UserManager, harness.RoleManager, harness.Configuration, harness.Logger);

        // Initial domain seed is skipped (existing data). The supplemental seeder now runs
        // as a BackgroundService after startup — SeedDatabase alone leaves instruments/genres empty.
        Assert.Equal(1, await harness.Context.Countries.CountAsync());
        Assert.Empty(await harness.Context.Instruments.ToListAsync());
        Assert.Empty(await harness.Context.Genres.ToListAsync());
        Assert.NotNull(await harness.UserManager.FindByNameAsync("System"));
    }

    private static SeedCounts ReadSeedCounts()
    {
        string countryPath = Path.Combine(AppContext.BaseDirectory, "countrySeeding.json");
        string instrumentPath = Path.Combine(AppContext.BaseDirectory, "instrumentSeeding.json");
        string genrePath = Path.Combine(AppContext.BaseDirectory, "genreSeeding.json");

        using JsonDocument countryDoc = JsonDocument.Parse(File.ReadAllText(countryPath));
        using JsonDocument instrumentDoc = JsonDocument.Parse(File.ReadAllText(instrumentPath));
        using JsonDocument genreDoc = JsonDocument.Parse(File.ReadAllText(genrePath));

        JsonElement genreRoot = genreDoc.RootElement;
        HashSet<string> validGenreIds = ExtractValidGenreIds(genreRoot);

        return new SeedCounts(
            countryDoc.RootElement.GetArrayLength(),
            instrumentDoc.RootElement.GetArrayLength(),
            genreRoot.GetProperty("genres").GetArrayLength(),
            CountDistinctValuesPerGenre(genreRoot, "genreAliases", "genreId", "alias", validGenreIds),
            CountDistinctValuesPerGenre(genreRoot, "genreSources", "genreId", "sourceLink", validGenreIds),
            CountDistinctPairs(genreRoot, "genreCountry", "genreId", "countryId"),
            CountDistinctPairs(genreRoot, "genreInstrument", "genreId", "instrumentId"),
            CountDistinctPairs(genreRoot, "genreHierarchy", "genreId", "subgenreId"),
            CountDistinctPairs(genreRoot, "genreSimilarity", "similarId1", "similarId2"),
            3);
    }

    private static HashSet<string> ExtractValidGenreIds(JsonElement genreRoot) =>
        genreRoot.GetProperty("genres")
            .EnumerateArray()
            .Select(item => item.TryGetProperty("id", out JsonElement idProp) ? idProp.GetString() : null)
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .Cast<string>()
            .ToHashSet(StringComparer.Ordinal);

    private static int CountDistinctPairs(JsonElement root, string propertyName, string leftPropertyName, string rightPropertyName)
    {
        if (!root.TryGetProperty(propertyName, out JsonElement element) || element.ValueKind != JsonValueKind.Array)
        {
            return 0;
        }

        HashSet<(string Left, string Right)> pairs = new();

        foreach (JsonElement item in element.EnumerateArray())
        {
            string? left = item.TryGetProperty(leftPropertyName, out JsonElement leftProperty) ? leftProperty.GetString() : null;
            string? right = item.TryGetProperty(rightPropertyName, out JsonElement rightProperty) ? rightProperty.GetString() : null;

            if (!string.IsNullOrWhiteSpace(left) && !string.IsNullOrWhiteSpace(right))
            {
                pairs.Add((left, right));
            }
        }

        return pairs.Count;
    }

    // Counts unique pairs across two seed files — deduplicating pairs that appear in both
    // (e.g. supplemental genre-instrument links that the original seed already contains).
    private static int CountDistinctPairsUnion(JsonElement root1, JsonElement root2, string propertyName, string leftPropertyName, string rightPropertyName)
    {
        HashSet<(string Left, string Right)> pairs = new();

        foreach (JsonElement root in (JsonElement[])[root1, root2])
        {
            if (!root.TryGetProperty(propertyName, out JsonElement element) || element.ValueKind != JsonValueKind.Array)
                continue;

            foreach (JsonElement item in element.EnumerateArray())
            {
                string? left = item.TryGetProperty(leftPropertyName, out JsonElement leftProp) ? leftProp.GetString() : null;
                string? right = item.TryGetProperty(rightPropertyName, out JsonElement rightProp) ? rightProp.GetString() : null;

                if (!string.IsNullOrWhiteSpace(left) && !string.IsNullOrWhiteSpace(right))
                    pairs.Add((left, right));
            }
        }

        return pairs.Count;
    }

    private static int CountDistinctValuesPerGenre(
        JsonElement root,
        string propertyName,
        string genrePropertyName,
        string valuePropertyName,
        HashSet<string> validGenreIds)
    {
        if (!root.TryGetProperty(propertyName, out JsonElement element) || element.ValueKind != JsonValueKind.Array)
        {
            return 0;
        }

        HashSet<(string GenreId, string Value)> entries = new();

        foreach (JsonElement item in element.EnumerateArray())
        {
            string? genreId = item.TryGetProperty(genrePropertyName, out JsonElement genreProperty) ? genreProperty.GetString() : null;
            string? value = item.TryGetProperty(valuePropertyName, out JsonElement valueProperty) ? valueProperty.GetString() : null;

            if (string.IsNullOrWhiteSpace(genreId) ||
                string.IsNullOrWhiteSpace(value) ||
                !validGenreIds.Contains(genreId))
            {
                continue;
            }

            entries.Add((genreId, value.ToUpperInvariant()));
        }

        return entries.Count;
    }

    private sealed record SeedCounts(
        int CountryCount,
        int InstrumentCount,
        int GenreCount,
        int AliasCount,
        int SourceCount,
        int GenreCountryCount,
        int GenreInstrumentCount,
        int GenreHierarchyCount,
        int GenreSimilarityCount,
        int RoleCount);

    private sealed class DbInitializerTestHarness : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly ServiceProvider _serviceProvider;
        private readonly IServiceScope _scope;

        public DbInitializerTestHarness()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            ServiceCollection services = new();
            services.AddLogging();
            services.AddDbContext<AppDbContext>(options => options.UseSqlite(_connection));
            services
                .AddIdentityCore<ApplicationUser>()
                .AddRoles<IdentityRole<Guid>>()
                .AddEntityFrameworkStores<AppDbContext>();

            _serviceProvider = services.BuildServiceProvider();
            _scope = _serviceProvider.CreateScope();

            Context = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
            UserManager = _scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            RoleManager = _scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            Logger = _scope.ServiceProvider.GetRequiredService<ILogger<DbInitializer>>();

            // Empty config — Admin:SeedEmails resolves to empty so the
            // admin seed no-ops.
            Configuration = new ConfigurationBuilder().Build();

            Context.Database.EnsureCreated();
        }

        public AppDbContext Context { get; }
        public UserManager<ApplicationUser> UserManager { get; }

        public RoleManager<IdentityRole<Guid>> RoleManager { get; }
        public ILogger<DbInitializer> Logger { get; }
        public IConfiguration Configuration { get; }

        public void Dispose()
        {
            _scope.Dispose();
            _serviceProvider.Dispose();
            _connection.Close();
            _connection.Dispose();
        }
    }
}
