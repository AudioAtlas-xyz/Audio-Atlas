using AudioAtlasDomain.MusicMetadata;
using AudioAtlasDomain.Geography;
using AudioAtlasDomain.Users;

namespace AudioAtlasDomain.Genres;

public class Genre
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid? AuthorId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int? StartYear { get; set; }

    public bool IsSensitive { get; set; } = false;

    public string? PlaylistLink { get; set; }

    public string? SensitiveDescription { get; set; }
    public ApplicationUser? Author { get; set; }

    public ICollection<ApplicationUser> Favoritees { get; set; } = new List<ApplicationUser>();

    public ICollection<GenreAlias> Aliases { get; set; } = new List<GenreAlias>();

    public ICollection<Country> Countries { get; set; } = new List<Country>();

    public ICollection<Instrument> Instruments { get; set; } = new List<Instrument>();

    public ICollection<GenreSource> Sources { get; set; } = new List<GenreSource>();

    public ICollection<Genre> SimilarGenres { get; set; } = new List<Genre>();

    public ICollection<Genre> SubGenres { get; set; } = new List<Genre>();

    public ICollection<Genre> ParentGenres { get; set; } = new List<Genre>();

}
