using AudioAtlasDomain.MusicMetadata;
using AudioAtlasDomain.Geography;

namespace AudioAtlasDomain.Genres;

public class Genre
{
    public Guid Id { get; set; }

    public Guid AuthorId { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public int? StartYear { get; set; }

    public bool IsSensitive { get; set; }

    public string PlaylistLink { get; set; }

    public string SensitiveDescription { get; set; }

    public ICollection<GenreAlias> Aliases { get; set; } = new List<GenreAlias>();

    public ICollection<Country> Countries { get; set; } = new List<Country>();

    public ICollection<Instrument> Instruments { get; set; } = new List<Instrument>();

    public ICollection<GenreSource> Sources { get; set; } = new List<GenreSource>();

    public ICollection<Genre> SimilarGenres { get; set; } = new List<Genre>();

    public ICollection<Genre> SubGenres { get; set; } = new List<Genre>();

    public ICollection<Genre> ParentGenres { get; set; } = new List<Genre>();

}