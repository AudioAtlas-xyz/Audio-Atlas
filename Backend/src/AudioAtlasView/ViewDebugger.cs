using AudioAtlasDomain.Genres;
using AudioAtlasDomain.Geography;
using AudioAtlasDomain.MusicMetadata;
using AudioAtlasInfrastructure.Database;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

            List<Genre> genres = context.Genres.ToList();
            List<Country> countries = context.Countries.ToList();
            List<Instrument> instruments = context.Instruments.ToList();


            if(File.Exists(file))
            {
                File.Delete(file);
            }

            string json = JsonSerializer.Serialize(genres, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.AppendAllText(file, json);


            json = JsonSerializer.Serialize(countries, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            
            json = JsonSerializer.Serialize(instruments, new JsonSerializerOptions
            {
                WriteIndented = true
            });

        }



    }
}
