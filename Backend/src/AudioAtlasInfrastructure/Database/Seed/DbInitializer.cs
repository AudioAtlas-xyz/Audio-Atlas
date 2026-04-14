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

            string countryPath = Path.Combine(AppContext.BaseDirectory, "countrySeeding.json");
            string instrumentPath = Path.Combine(AppContext.BaseDirectory, "instrumentSeeding.json");
            string genrePath = Path.Combine(AppContext.BaseDirectory, "genreSeeding.json");

            logger.LogInformation("Loading seed data from {SeedPath}", genrePath);

            string countryJson = File.ReadAllText(countryPath);
            string instrumentJson = File.ReadAllText(instrumentPath);
            string genreJson = File.ReadAllText(genrePath);

            Dictionary<string, Country> countryMapping = null;
            Dictionary<string, Instrument> instrumentMapping = null;

            using (JsonDocument doc = JsonDocument.Parse(countryJson))
            {
                JsonElement root = doc.RootElement;


                countryMapping = ProcessCountries(root, logger);
                dbContext.Countries.AddRange(countryMapping.Values);
            }

            using (JsonDocument doc = JsonDocument.Parse(instrumentJson))
            {
                JsonElement root = doc.RootElement;
                instrumentMapping = ProcessInstruments(root, logger);

                dbContext.Instruments.AddRange(instrumentMapping.Values);

            }


            using (JsonDocument doc = JsonDocument.Parse(genreJson))
            {
                JsonElement root = doc.RootElement;
                JsonElement genres = root.GetProperty("genres");

                Dictionary<string, Genre> genreMapping = ProcessGenres(genres, countryMapping, instrumentMapping, logger);

                ProcessGenreRelationships(genres, genreMapping, logger);

                dbContext.SaveChanges();

            }


        }

        private static Dictionary<string, Country> ProcessCountries(JsonElement countryRoot, ILogger<DbInitializer> logger)
        {
            Dictionary<string, Country> countryMapping = new Dictionary<string, Country>();
            int countryCount = 0;

            foreach (JsonElement obj in countryRoot.EnumerateArray())
            {


                string? id = obj.GetProperty("Id").GetString();
                string? name = obj.GetProperty("Name").GetString();
                string? region = obj.GetProperty("Region").GetString();
                string? continent = obj.GetProperty("Continent").GetString();
                string? desc = obj.GetProperty("Description").GetString();

                if (string.IsNullOrWhiteSpace(id))
                {
                    logger.LogWarning("Skipping seed country with no id.");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(region))
                {
                    logger.LogWarning("Skipping seed country with no region.");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(continent))
                {
                    logger.LogWarning("Skipping seed country with no continent.");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(desc))
                {
                    logger.LogWarning("Skipping seed country with no description.");
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
                    Name = name,
                    Description = desc,
                    Region = region,
                    Continent = continent
                };

                logger.LogInformation(
                    "Prepared seed country {CountryName} from source id {SourceId} with generated id {CountryId}. Has region {Region}, continent {Continent} and Description {Desc}",
                    country.Name,
                    id,
                    country.Id,
                    country.Region,
                    country.Continent,
                    country.Description);

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

            foreach (JsonElement obj in instrumentRoot.EnumerateArray())
            {
                string? id = obj.GetProperty("Id").GetString();
                string? name = obj.GetProperty("Name").GetString();
                string? type = obj.GetProperty("Type").GetString();
                string? desc = obj.GetProperty("Description").GetString();

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

                if (string.IsNullOrWhiteSpace(desc))
                {
                    logger.LogWarning("Skipping seed country with no description.");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(type))
                {
                    logger.LogWarning("Skipping seed country with no type.");
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
                    "Prepared seed instrument {InstrumentName} from source id {SourceId} with generated id {Instrumentd}, type {Type} and description {Description}",
                    instrument.Type,
                    id,
                    instrument.Id,
                    type,
                    desc);

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

                    if(!countryMapping.ContainsKey(countryElementID))
                    {
                        continue;
                    }

                    Country originCountry = countryMapping[countryElementID];

                    genre.Countries.Add(originCountry);

                    originCountry.Genres.Add(genre);

                    logger.LogInformation(
                    "Added relationship between Genre: {GenreName} with GenreID {GenreID} and Country: {CountryName} with CountryID {CountryId}",
                    genre.Name,
                    id,
                    originCountry.Name,
                    countryElementID);

                }

                foreach (JsonElement instrumentElement in instruments.EnumerateArray())
                {

                    string? instrumentElementID = instrumentElement.GetString();


                    if (string.IsNullOrWhiteSpace(instrumentElementID))
                    {
                        continue;
                    }

                    if (!instrumentMapping.ContainsKey(instrumentElementID))
                    {
                        continue;
                    }

                    Instrument instrumentEntity = instrumentMapping[instrumentElementID];

                    genre.Instruments.Add(instrumentEntity);

                    instrumentEntity.Genres.Add(genre);

                    logger.LogInformation(
                    "Added relationship between Genre: {GenreName} with GenreID {GenreID} and Instrument: {InstrumentName} with InstrumentID {InstrumentId}",
                    genre.Name,
                    id,
                    instrumentEntity.Type,
                    instrumentElementID);

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
