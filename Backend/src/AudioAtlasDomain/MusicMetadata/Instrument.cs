using AudioAtlasDomain.Genres;

namespace AudioAtlasDomain.MusicMetadata
{
    public class Instrument
    {
        public Guid Id { get; set; }

        public string Type { get; set; } = string.Empty;

        public string? Description { get; set; }

        public ICollection<Genre> Genres { get; set; } = new List<Genre>();
    }
}
