namespace Domain.Genre;

public class Genre
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public string Description { get; set; }
    public required ICollection<string> Origin { get; set; } = new List<string>();
    public ICollection<string>? Aliases { get; set; }
    public ICollection<Genre> RelatedGenres { get; set; } = new List<Genre>();
}