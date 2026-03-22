using AudioAtlasInfrastructure.Database;
using System.Text.Json;

namespace AudioAtlasView
{
    public class ViewDebugger
    {
        public static void DebugToFile(AppDbContext context)
        {
            string directory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "AudioAtlasDebug");

            Directory.CreateDirectory(directory);

            string file = Path.Combine(directory, "output.json");

            var genres = context.Genres
                .Select(g => new
                {
                    g.Id,
                    g.Name,
                    g.Description,
                    g.StartYear,
                    g.IsSensitive,
                    CountryIds = g.Countries.Select(c => c.Id).ToList(),
                    InstrumentIds = g.Instruments.Select(i => i.Id).ToList(),
                    ParentGenreIds = g.ParentGenres.Select(parent => parent.Id).ToList(),
                    SubGenreIds = g.SubGenres.Select(child => child.Id).ToList(),
                    SimilarGenreIds = g.SimilarGenres.Select(similar => similar.Id).ToList()
                })
                .ToList();

            var countries = context.Countries
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.Description,
                    GenreIds = c.Genres.Select(g => g.Id).ToList()
                })
                .ToList();

            var instruments = context.Instruments
                .Select(i => new
                {
                    i.Id,
                    i.Type,
                    i.Description,
                    GenreIds = i.Genres.Select(g => g.Id).ToList()
                })
                .ToList();

            var payload = new
            {
                Genres = genres,
                Countries = countries,
                Instruments = instruments
            };

            string json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(file, json);
        }
    }
}
