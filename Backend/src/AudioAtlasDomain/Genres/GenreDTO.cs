using AudioAtlasDomain.Geography;
using AudioAtlasDomain.MusicMetadata;

namespace AudioAtlasDomain.Genres;

public class GenreDTO
{
    public Guid Id { get; set; }
    public Guid? AuthorId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? Summary { get; set; }
    public int? StartYear { get; set; }
    public bool IsSensitive { get; set; }
    public string? SensitiveDescription { get; set; }
    public string? PlaylistLink { get; set; }
    public ICollection<CountryDTO> Countries { get; set; } = new List<CountryDTO>();
    public ICollection<InstrumentDTO> Instruments { get; set; } = new List<InstrumentDTO>();
    public ICollection<GenreAliasDTO> Aliases { get; set; } = new List<GenreAliasDTO>();
    public ICollection<GenreSourceDTO> Sources { get; set; } = new List<GenreSourceDTO>();
    public ICollection<GenreDTO> SimilarGenres { get; set; } = new List<GenreDTO>();
    public ICollection<GenreDTO> SubGenres { get; set; } = new List<GenreDTO>();
    public ICollection<GenreDTO> ParentGenres { get; set; } = new List<GenreDTO>();
}
