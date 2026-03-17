using AudioAtlasDomain.Genres;
using AudioAtlasDomain.Submissions;

namespace AudioAtlasDomain.Geography
{
    public class Country
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public ICollection<Genre> Genres { get; set; } = new List<Genre>();
        public ICollection<Submission> Submissions { get; set; } = new List<Submission>(); 
    }
}