namespace AudioAtlasApplication.Services;
using AudioAtlasDomain.Genres;

public interface IGenreService
{
    public GenreDTO getGenre(Guid id);
}