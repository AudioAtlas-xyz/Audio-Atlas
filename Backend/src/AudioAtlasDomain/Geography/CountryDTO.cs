using AudioAtlasDomain.Submissions;

namespace AudioAtlasDomain.Geography;

using Genres;

public class CountryDTO
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public ICollection<GenreDTO> Genres { get; set; } = new List<GenreDTO>();
    public ICollection<Submission> Submissions { get; set; } = new List<Submission>(); 


}