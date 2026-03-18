using AudioAtlasDomain.MusicMetadata;
using AudioAtlasDomain.Geography;
using AudioAtlasDomain.Users;

namespace AudioAtlasDomain.Genres;

/// <summary>
/// Represents a musical genre with associated descriptive metadata,
/// cultural context, and relationships to other genres.
/// </summary>
public class Genre
{
    /// <summary>
    /// Unique identifier for the genre.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Identifier of the user who created the genre entry.
    /// Nullable for system-defined genres.
    /// </summary>
    public Guid? AuthorId { get; set; }

    /// <summary>
    /// The canonical name of the genre.
    /// This should be the most widely recognized or accepted name.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// A general description of the genre, including stylistic traits,
    /// historical context, and defining characteristics.
    /// 
    /// This field should aim to answer:
    /// "What makes this genre distinct from others?"
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Approximate year the genre originated.
    /// 
    /// This is not expected to be exact, but should reflect the
    /// commonly accepted emergence period of the genre.
    /// </summary>
    public int? StartYear { get; set; }

    /// <summary>
    /// Indicates whether the genre contains themes that may be considered
    /// sensitive (e.g., explicit content, controversial cultural context).
    /// 
    /// Used to gate or flag content in user-facing applications.
    /// </summary>
    public bool IsSensitive { get; set; } = false;

    /// <summary>
    /// External reference representing the genre (e.g., a curated playlist).
    /// 
    /// Intended to give a direct experiential example of the genre's sound.
    /// Not guaranteed to be permanent or authoritative.
    /// </summary>
    public string? PlaylistLink { get; set; }

    /// <summary>
    /// Additional explanation of why the genre is marked as sensitive.
    /// 
    /// This should provide context rather than labels,
    /// helping users understand the nature of the sensitivity.
    /// </summary>
    public string? SensitiveDescription { get; set; }

    /// <summary>
    /// Navigation property to the user who created the genre.
    /// </summary>
    public ApplicationUser? Author { get; set; }

    /// <summary>
    /// Users who have marked this genre as a favorite.
    /// Represents user preference rather than domain knowledge.
    /// </summary>
    public ICollection<ApplicationUser> Favoritees { get; set; } = new List<ApplicationUser>();

    /// <summary>
    /// Alternative names for the genre.
    /// 
    /// Useful for handling regional naming differences, historical terms,
    /// or synonymous labels used across communities.
    /// </summary>
    public ICollection<GenreAlias> Aliases { get; set; } = new List<GenreAlias>();

    /// <summary>
    /// Countries associated with the genre.
    /// 
    /// This may represent origin, cultural significance, or regions
    /// where the genre has had notable development.
    /// Not limited to a single country.
    /// </summary>
    public ICollection<Country> Countries { get; set; } = new List<Country>();

    /// <summary>
    /// Instruments commonly associated with the genre.
    /// 
    /// These are typical, not required—genres may evolve beyond them.
    /// Helps define the sonic identity of the genre.
    /// </summary>
    public ICollection<Instrument> Instruments { get; set; } = new List<Instrument>();

    /// <summary>
    /// Sources supporting the information about this genre.
    /// 
    /// Intended to provide traceability and credibility
    /// for genre definitions and relationships.
    /// </summary>
    public ICollection<GenreSource> Sources { get; set; } = new List<GenreSource>();

    /// <summary>
    /// Genres that are stylistically similar.
    /// Non-hierarchical relationship used for discovery and exploration.
    /// </summary>
    public ICollection<Genre> SimilarGenres { get; set; } = new List<Genre>();

    /// <summary>
    /// More specific genres derived from this genre.
    /// </summary>
    public ICollection<Genre> SubGenres { get; set; } = new List<Genre>();

    /// <summary>
    /// Broader genres that this genre belongs to.
    /// </summary>
    public ICollection<Genre> ParentGenres { get; set; } = new List<Genre>();

}
