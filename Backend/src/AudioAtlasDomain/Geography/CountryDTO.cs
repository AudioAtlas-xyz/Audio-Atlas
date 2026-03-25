using AudioAtlasDomain.Submissions;

namespace AudioAtlasDomain.Geography;

using Genres;

public class CountryDTO
{
    /// <summary>
    /// Unique identifier for the country.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// Name of the country.
    /// </summary>
    public string Name { get; set; } = null!;
    
    /// <summary>
    /// A general description of the country.
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Genres originating from the country.
    /// </summary>
    public ICollection<GenreDTO> Genres { get; set; } = new List<GenreDTO>();
    
    /// <summary>
    /// User submissions related to this country.
    /// </summary>
    public ICollection<Submission> Submissions { get; set; } = new List<Submission>(); 

}