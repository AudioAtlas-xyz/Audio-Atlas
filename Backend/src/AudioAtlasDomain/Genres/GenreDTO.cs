namespace AudioAtlasDomain.Genres;
using Geography;

public class GenreDTO
{
    public Guid Id { get; set; }
    public Guid? AuthorId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    
    public int? StartYear { get; set; }
    public bool IsSensitive { get; set; } = false;
    public string? SensitiveDescription { get; set; }
    
    public string? PlaylistLink { get; set; }
    
    public ICollection<CountryDTO> Countries { get; set; } = new List<CountryDTO>();

    public ICollection<GenreAlias>? Aliases { get; set; }
    public ICollection<GenreDTO> SimilarGenres { get; set; } = new List<GenreDTO>();
    public ICollection<GenreDTO> SubGenres { get; set; } = new List<GenreDTO>();
    public ICollection<GenreDTO> ParentGenres { get; set; } = new List<GenreDTO>();
}