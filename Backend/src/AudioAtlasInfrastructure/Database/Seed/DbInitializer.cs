using AudioAtlasDomain.Genres;
using AudioAtlasDomain.Geography;
using AudioAtlasDomain.MusicMetadata;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Linq;

namespace AudioAtlasInfrastructure.Database.Seed
{
    public class DbInitializer
    {

        public static void SeedDatabase(AppDbContext dbContext, ILogger<DbInitializer> logger)
        {

            if (dbContext.Instruments.Any() || dbContext.Genres.Any() || dbContext.Countries.Any())
            {
                logger.LogWarning("Skipping seeding as database contains data.");
                return;
            }

            string path = Path.Combine(AppContext.BaseDirectory, "seeding.json");
            logger.LogInformation("Loading seed data from {SeedPath}", path);

            string json = File.ReadAllText(path);


            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                JsonElement root = doc.RootElement;
                JsonElement countries = root.GetProperty("lookups").GetProperty("countries");
                JsonElement instruments = root.GetProperty("lookups").GetProperty("instruments");
                JsonElement genres = root.GetProperty("genres");

                Dictionary<string, Country> countryMapping = ProcessCountries(countries, logger);
                Dictionary<string, Instrument> instrumentMapping = ProcessInstruments(instruments, logger);
                Dictionary<string, Genre> genreMapping = ProcessGenres(genres, countryMapping, instrumentMapping, logger);

                ProcessGenreRelationships(genres, genreMapping, logger);

                dbContext.Countries.AddRange(countryMapping.Values);
                dbContext.Instruments.AddRange(instrumentMapping.Values);
                dbContext.Genres.AddRange(genreMapping.Values);
                dbContext.SaveChanges();

            }


        }

        private static Dictionary<string, Country> ProcessCountries(JsonElement countryRoot, ILogger<DbInitializer> logger)
        {
            Dictionary<string, Country> countryMapping = new Dictionary<string, Country>();
            int countryCount = 0;

            foreach (JsonProperty property in countryRoot.EnumerateObject())
            {
                JsonElement obj = property.Value;

                string? id = obj.GetProperty("id").GetString();
                string? name = obj.GetProperty("name").GetString();

                if (string.IsNullOrWhiteSpace(id))
                {
                    logger.LogWarning("Skipping seed country with no id.");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(name))
                {
                    logger.LogWarning("Skipping seed country with source id {SourceId} because it has no name.", id);
                    continue;
                }

                if (countryMapping.ContainsKey(id))
                {

                    logger.LogWarning("Skipping seed country with source id {SourceId} because it is a duplicate.", id);
                    continue;
                }

                Country country = new Country
                {
                    Name = name
                };

                logger.LogInformation(
                    "Prepared seed country {CountryName} from source id {SourceId} with generated id {CountryId}",
                    country.Name,
                    id,
                    country.Id);

                countryMapping.Add(id, country);

                countryCount++;
            }

            logger.LogInformation("Processed {CountryCount} countries from seed data", countryCount);

            return countryMapping;
        }

        private static Dictionary<string, Instrument> ProcessInstruments(JsonElement instrumentRoot, ILogger<DbInitializer> logger)
        {
            int instrumentCount = 0;
            Dictionary<string, Instrument> instrumentMapping = new Dictionary<string, Instrument>();

            foreach (JsonProperty property in instrumentRoot.EnumerateObject())
            {
                JsonElement obj = property.Value;

                string? id = obj.GetProperty("id").GetString();
                string? name = obj.GetProperty("name").GetString();

                if (string.IsNullOrWhiteSpace(id))
                {
                    logger.LogWarning("Skipping seed instrument with no id.");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(name))
                {
                    logger.LogWarning("Skipping seed instrument with source id {SourceId} because it has no name.", id);
                    continue;
                }

                if (instrumentMapping.ContainsKey(id))
                {

                    logger.LogWarning("Skipping seed instrument with source id {SourceId} because it is a duplicate.", id);
                    continue;
                }

                Instrument instrument = new Instrument
                {
                    Type = name
                };


                logger.LogInformation(
                    "Prepared seed instrument {InstrumentName} from source id {SourceId} with generated id {Instrumentd}",
                    instrument.Type,
                    id,
                    instrument.Id);

                instrumentMapping.Add(id, instrument);

                instrumentCount++;
            }

            logger.LogInformation("Processed {InstrumentCount} countries from seed data", instrumentCount);
            return instrumentMapping;

        }

        private static Dictionary<string, Genre> ProcessGenres(JsonElement genreRoot, Dictionary<string, Country> countryMapping, Dictionary<string, Instrument> instrumentMapping, ILogger<DbInitializer> logger)
        {
            int genreCount = 0;
            Dictionary<string, Genre> GenreMapping = new Dictionary<string, Genre>();

            foreach (JsonElement property in genreRoot.EnumerateArray())
            {

                string? id = property.GetProperty("id").GetString();
                string? name = property.GetProperty("name").GetString();
                JsonElement countryOrigins = property.GetProperty("origin").GetProperty("country_ids");
                JsonElement instruments = property.GetProperty("instruments").GetProperty("instrument_ids");

                int? startYear = int.TryParse(
                    property.GetProperty("period").GetProperty("approx_start").ToString(),
                    out int parsedYear) ? parsedYear : null;

                if (string.IsNullOrWhiteSpace(id))
                {
                    logger.LogWarning("Skipping seed genre with no id.");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(name))
                {
                    logger.LogWarning("Skipping seed instrument with source id {SourceId} because it has no name.", id);
                    continue;
                }

                if (GenreMapping.ContainsKey(id))
                {
                    logger.LogWarning("Skipping seed genre with source id {SourceId} because it is a duplicate.", id);
                    continue;
                }

                Genre genre = new Genre
                {
                    Name = name,
                    StartYear = startYear
                };

                foreach (JsonElement countryElement in countryOrigins.EnumerateArray())
                {

                    string? countryElementID = countryElement.GetString();


                    if (string.IsNullOrWhiteSpace(countryElementID))
                    {
                        continue;
                    }

                    Country originCountry = countryMapping[countryElementID];

                    genre.Countries.Add(originCountry);

                    originCountry.Genres.Add(genre);

                }

                foreach (JsonElement instrumentElement in instruments.EnumerateArray())
                {

                    string? instrumentElementID = instrumentElement.GetString();


                    if (string.IsNullOrWhiteSpace(instrumentElementID))
                    {
                        continue;
                    }

                    Instrument instrumentEntity = instrumentMapping[instrumentElementID];

                    genre.Instruments.Add(instrumentEntity);

                    instrumentEntity.Genres.Add(genre);

                }

                logger.LogInformation(
                    "Prepared seed instrument {GenreName} from source id {SourceId} with generated id {GenreId}",
                    genre.Name,
                    id,
                    genre.Id);

                GenreMapping.Add(id, genre);

                genreCount++;
            }

            logger.LogInformation("Processed {InstrumentCount} countries from seed data", genreCount);
            return GenreMapping;

        }


        private static void ProcessGenreRelationships(JsonElement genreRoot, Dictionary<string, Genre> genreMapping, ILogger<DbInitializer> logger)
        {
            int relationshipMappingCount = 0;

            foreach (JsonElement property in genreRoot.EnumerateArray())
            {

                string? id = property.GetProperty("id").GetString();


                if (string.IsNullOrWhiteSpace(id))
                {
                    continue;
                }

                Genre mainGenre = genreMapping[id];

                JsonElement lineage = property.GetProperty("lineage");

                JsonElement parent = lineage.GetProperty("parent_ids");

                JsonElement predecessor = lineage.GetProperty("predecessor_ids");

                JsonElement similar = lineage.GetProperty("similar_ids");
                JsonElement subSengre = lineage.GetProperty("subgenre_ids");

                List<string?> predecessors =
                    lineage.GetProperty("predecessor_ids")
                        .EnumerateArray()
                        .Select(x => x.GetString())
                    .Concat(
                        lineage.GetProperty("parent_ids")
                            .EnumerateArray()
                            .Select(x => x.GetString())
                    )
                    .ToList();

                foreach (string parentElementID in predecessors)
                {

                    if (string.IsNullOrWhiteSpace(parentElementID))
                    {
                        continue;
                    }

                    Genre parentGenre = genreMapping[parentElementID];

                    if (!mainGenre.ParentGenres.Any(x => x.Id == parentGenre.Id))
                    {
                        mainGenre.ParentGenres.Add(parentGenre);
                        relationshipMappingCount++;
                    }

                    if (!parentGenre.SubGenres.Any(x => x.Id == mainGenre.Id))
                    {
                        parentGenre.SubGenres.Add(mainGenre);
                        relationshipMappingCount++;
                    }

                }

                foreach (JsonElement subGenreElement in subSengre.EnumerateArray())
                {

                    var subGenreID = subGenreElement.GetString();

                    if (string.IsNullOrWhiteSpace(subGenreID))
                    {
                        continue;
                    }

                    Genre subGenre = genreMapping[subGenreID];

                    if (!mainGenre.SubGenres.Any(x => x.Id == subGenre.Id))
                    {
                        mainGenre.SubGenres.Add(subGenre);
                        relationshipMappingCount++;
                    }

                    if (!subGenre.ParentGenres.Any(x => x.Id == mainGenre.Id))
                    {
                        subGenre.ParentGenres.Add(mainGenre);
                        relationshipMappingCount++;
                    }

                }

                foreach (JsonElement similarGenreElement in similar.EnumerateArray())
                {

                    var similarGenreID = similarGenreElement.GetString();

                    if (string.IsNullOrWhiteSpace(similarGenreID))
                    {
                        continue;
                    }

                    Genre similarGenre = genreMapping[similarGenreID];

                    if (!mainGenre.SimilarGenres.Any(x => x.Id == similarGenre.Id))
                    {
                        mainGenre.SimilarGenres.Add(similarGenre);
                        relationshipMappingCount++;
                    }

                    if (!similarGenre.SimilarGenres.Any(x => x.Id == mainGenre.Id))
                    {
                        similarGenre.SimilarGenres.Add(mainGenre);
                        relationshipMappingCount++;
                    }

                }
            }

            logger.LogInformation("Processed {RelationshipMappingCount} genre Relationships from seed data", relationshipMappingCount);
        }
    }
}
