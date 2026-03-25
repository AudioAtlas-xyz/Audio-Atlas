namespace AudioAtlasDomain.Genres;
using Geography;

public class GenreDTO
{
    /// <summary>
    /// Unique identifier for the genre.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Unique identifier for the genres author.
    /// </summary>
    public Guid? AuthorId { get; set; }
    
    /// <summary>
    /// The name of the genre
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// A general description of the genre.
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// When the genre originated.
    /// </summary>
    public int? StartYear { get; set; }
    
    /// <summary>
    /// Tells whether a genre is culturally sensitive or not.
    /// </summary>
    public bool IsSensitive { get; set; } = false;
    
    /// <summary>
    /// Description of why a genre is sensitive, if it is sensitive.
    /// </summary>
    public string? SensitiveDescription { get; set; }
    
    /// <summary>
    /// Link to a playlist of songs related to the genre
    /// </summary>
    public string? PlaylistLink { get; set; }
    
    /// <summary>
    /// Countries associated with the genre
    /// </summary>
    public ICollection<CountryDTO> Countries { get; set; } = new List<CountryDTO>();

    /// <summary>
    /// Alternative names for the genre.
    /// </summary>
    public ICollection<GenreAlias>? Aliases { get; set; }
    
    /// <summary>
    /// Genres that are similar to the genre.
    /// </summary>
    public ICollection<GenreDTO> SimilarGenres { get; set; } = new List<GenreDTO>();
    
    /// <summary>
    /// The subgenres of the genre.
    /// </summary>
    public ICollection<GenreDTO> SubGenres { get; set; } = new List<GenreDTO>();
    
    /// <summary>
    /// The parents of the genre.
    /// </summary>
    public ICollection<GenreDTO> ParentGenres { get; set; } = new List<GenreDTO>();
}