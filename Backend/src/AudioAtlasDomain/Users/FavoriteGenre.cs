using AudioAtlasDomain.Genres;

using System.ComponentModel.DataAnnotations;
using AudioAtlasDomain.Genres;
namespace AudioAtlasDomain.Users;
public class FavoriteGenre
{
    [Key]
    public Guid UserId { get; set; }

    public Guid GenreId { get; set; }

    public Genre Genre { get; set; } = null!;
}
