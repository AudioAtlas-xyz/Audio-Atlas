namespace AudioAtlasApplication.Repositories;
using AudioAtlasDomain.Genres;

public interface IGenreRepository
{
    public Genre getGenre(Guid id);
    public Task<ICollection<Genre>> getGenresByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken = default);
    public string getName(Guid id);
    public ICollection<GenreAlias> getAliases(Guid id);
    public ICollection<Genre> getParents(Guid id);
    public ICollection<Genre> getSubGenres(Guid id);
    public ICollection<Genre> getSimilarGenres(Guid id);
    public string getDescription(Guid id);
    public ICollection<Genre> getRelated(Guid id);
    public ICollection<Genre> getAllGenres();
    public ICollection<Genre> getGenresByAuthorId(Guid id);
    public Task<ICollection<Genre>> SearchForGenres(string keyword);
    public Task SaveChangesAsync();
}
