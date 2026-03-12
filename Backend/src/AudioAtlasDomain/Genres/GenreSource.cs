namespace AudioAtlasDomain.Genres;

public class GenreSource
{
    public Guid Id { get; set; }

    public string SourceLink { get; set; }

    public Guid GenreId { get; set; }
    
    public Genre Genre { get; set; } = null!;
}