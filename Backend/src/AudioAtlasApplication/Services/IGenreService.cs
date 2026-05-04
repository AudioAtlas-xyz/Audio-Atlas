namespace AudioAtlasApplication.Services;
using AudioAtlasDomain.Genres;

public interface IGenreService
{
    public GenreDTO? GetGenre(Guid id);

    public ICollection<GenreDTO> GetAllGenres();

    public Task<ICollection<GenreDTO>> SearchForGenres(string keyword);

}