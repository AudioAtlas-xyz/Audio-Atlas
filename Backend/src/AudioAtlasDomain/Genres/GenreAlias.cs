namespace AudioAtlasDomain.Genres;

public class GenreAlias
{
    public Guid Id { get; set; }

    public string Alias { get; set; }

    public Guid GenreId { get; set; }

    public Genre Genre { get; set; }
}