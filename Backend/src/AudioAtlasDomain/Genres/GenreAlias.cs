namespace AudioAtlasDomain.Genres;

/// <summary>
/// Represents an alternative name for a genre.
/// 
/// Aliases are used to capture variations in naming across regions,
/// time periods, or communities. They allow different labels to resolve
/// to the same underlying genre entity.
/// 
/// This helps prevent duplication while supporting flexible search
/// and representation of genre terminology.
/// </summary>
public class GenreAlias
{
    /// <summary>
    /// The alternative name of the genre.
    /// 
    /// This may include slang, historical names, translations,
    /// or stylistic variations of the canonical genre name.
    /// </summary>
    public string Alias { get; set; }

    /// <summary>
    /// Foreign key referencing the associated genre.
    /// </summary>
    public Guid GenreId { get; set; }

    /// <summary>
    /// Navigation property to the genre this alias refers to.
    /// 
    /// Multiple aliases can map to the same genre,
    /// but each alias belongs to exactly one genre.
    /// </summary>
    public Genre Genre { get; set; }
}