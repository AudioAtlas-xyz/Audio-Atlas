namespace AudioAtlasApplication.Repositories;
using AudioAtlasDomain.Genres;

public interface IGenreRepository
{
    public Genre getGenre(Guid id);
    public string getName(Guid id);
    public ICollection<GenreAlias> getAliases(Guid id);
    public ICollection<Genre> getParents(Guid id);
    public ICollection<Genre> getSubGenres(Guid id);
    public ICollection<Genre> getSimilarGenres(Guid id);
    public string getDescription(Guid id);
    public ICollection<Genre> getRelated(Guid id);
    public ICollection<Genre> getAllGenres();
}