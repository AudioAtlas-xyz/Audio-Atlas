using AudioAtlasDomain.Geography;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AudioAtlasInfrastructure.Database.Seed
{
    public class DbInitializer
    {
        public static void SeedDatabase(AppDbContext dbContext, ILogger<DbInitializer> logger)
        {
            string path = Path.Combine(AppContext.BaseDirectory, "seeding.json");
            logger.LogInformation("Loading seed data from {SeedPath}", path);

            string json = File.ReadAllText(path);
            Dictionary<string, Guid> countryMapping = new Dictionary<string, Guid>();

            JsonDocument doc = JsonDocument.Parse(json);
            JsonElement root = doc.RootElement;
            JsonElement countries = root.GetProperty("lookups").GetProperty("countries");
            JsonElement instruments = root.GetProperty("lookups").GetProperty("instruments");
            JsonElement genres = root.GetProperty("genres");
            var countryCount = 0;

            foreach (JsonProperty property in countries.EnumerateObject())
            {
                JsonElement obj = property.Value;

                string? id = obj.GetProperty("id").GetString();
                string? name = obj.GetProperty("name").GetString();

                if (string.IsNullOrWhiteSpace(name))
                {
                    logger.LogWarning("Skipping seed country with source id {SourceId} because it has no name.", id);
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

                countryCount++;
            }

            logger.LogInformation("Processed {CountryCount} countries from seed data", countryCount);
        }
    }
}
