namespace AudioAtlasApplication.Repositories;
using AudioAtlasDomain.Genres;

public interface IGenreRepository
{
    public Genre getGenre(Guid id);
    public string getName(Genre genre);
    public ICollection<GenreAlias> getAliases(Genre genre);
    public ICollection<Genre> getParents(Genre genre);
    public ICollection<Genre> getSubGenres(Genre genre);
    public ICollection<Genre> getSimilarGenres(Genre genre);
    public string getDescription(Genre genre);
    public ICollection<Genre> getRelated(Genre genre);
}