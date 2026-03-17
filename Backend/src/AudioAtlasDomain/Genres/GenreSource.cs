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

    public Guid GenreId { get; set; }
    
    public Genre Genre { get; set; } = null!;
}