using AudioAtlasDomain.Genres;

using AudioAtlasDomain.Genres;
namespace AudioAtlasDomain.Users;
public class FavoriteGenre
{
    public Guid UserId { get; set; }

    public Guid GenreId { get; set; }

    public Genre Genre { get; set; } = null!;
}
