namespace AudioAtlasDomain.Genres;

/// <summary>
/// Represents an external source supporting information about a genre.
/// 
/// Sources provide traceability and justification for genre definitions,
/// relationships, and metadata. They link the internal data model to
/// external references such as articles, databases, or curated content.
/// 
/// A source does not guarantee correctness, but offers context and
/// evidence for how the genre is described within the system.
/// </summary>
public class GenreSource
{
    /// <summary>
    /// URL or reference to the external source.
    /// 
    /// This may point to articles, academic material, music platforms,
    /// or other resources describing the genre.
    /// 
    /// Stability and availability of the link are not guaranteed.
    /// </summary>
    public string SourceLink { get; set; }

    /// <summary>
    /// Foreign key referencing the associated genre.
    /// </summary>
    public Guid GenreId { get; set; }

    /// <summary>
    /// Navigation property to the genre this source supports.
    /// 
    /// Multiple sources can be associated with a single genre,
    /// forming a basis for validation and further exploration.
    /// </summary>
    public Genre Genre { get; set; } = null!;
}