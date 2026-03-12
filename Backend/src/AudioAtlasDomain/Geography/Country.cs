using AudioAtlasDomain.Genres;

namespace AudioAtlasDomain.Geography
{
    public class Country
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public ICollection<Genre> Genres { get; set; } = new List<Genre>();
    }
}