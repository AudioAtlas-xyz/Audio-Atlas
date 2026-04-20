using AudioAtlasDomain.Genres;
using AudioAtlasDomain.Geography;
using AudioAtlasDomain.MusicMetadata;
using AudioAtlasDomain.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AudioAtlasInfrastructure.Database.Seed;

public class DbInitializer
{
    public static async Task SeedDatabase(AppDbContext dbContext, UserManager<ApplicationUser> userManager,ILogger<DbInitializer> logger)
    {
        if (dbContext.Instruments.Any() || dbContext.Genres.Any() || dbContext.Countries.Any())
        {
            logger.LogWarning("Skipping seeding as database contains data.");
            return;
        }

            string path = Path.Combine(AppContext.BaseDirectory, "seeding.json");
            logger.LogInformation("Loading seed data from {SeedPath}", path);

        if (systemUser == null)
        {
            systemUser = new ApplicationUser
            {
                UserName = "system",
                Email = "system@audioatlas.com",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            // Create without password using a bypass
            var result = await userManager.CreateAsync(systemUser);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    logger.LogError("Identity error: {Code} - {Description}", error.Code, error.Description);
                }

                throw new Exception($"Could not create system user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            await dbContext.SaveChangesAsync();
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
                Summary = TryGetStringProperty(obj, "summary"),
                StartYear = ParseStartYear(obj),
                IsSensitive = TryGetBoolProperty(obj, "isSensitive"),
                SensitiveDescription = TryGetStringProperty(obj, "sensitiveDescription"),
                PlaylistLink = TryGetStringProperty(obj, "playlistLink"),
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
