using AudioAtlasDomain.Genres;

namespace AudioAtlasDomain.MusicMetadata
{
    public class Instrument
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Type { get; set; } = null!;

        public string? Description { get; set; }

        public ICollection<Genre> Genres { get; set; } = new List<Genre>();
    }
}
