using AudioAtlasApplication.Repositories;
using AudioAtlasDomain.Genres;
using AudioAtlasInfrastructure.Database;

namespace AudioAtlasInfrastructure.Repositories;

public class GenreRepository : IGenreRepository
{
    readonly AppDbContext _dbcontext;

    public GenreRepository(AppDbContext appDbContext)
    {
        _dbcontext = appDbContext;
    }
    
    /// <summary>
    /// Retrieves the description of a genre from the database
    /// </summary>
    /// <param name="genre"> Specific genre </param>
    /// <returns> The description of a specific genre or null </returns>
    public string getDescription(Genre genre)
    {
        return _dbcontext.Genres
            .Where(g => g.Id == genre.Id)
            .First()
            .Description;
    }

    /// <summary>
    /// Retrieves the name corresponding to the genre
    /// </summary>
    /// <param name="genre"> Specific genre </param>
    /// <returns> A string corresponding to the genres name </returns>
    public string getName(Genre genre)
    {
        return _dbcontext.Genres
            .Where(g => g.Id == genre.Id)
            .First()
            .Name;
    }

    /// <summary>
    /// Retrieves a country from the database based on id
    /// </summary>
    /// <param name="id"> Unique identifier for a specific genre </param>
    /// <returns> The genre corresponding to the ID </returns>
    public Genre getGenre(Guid id)
    {
        return _dbcontext.Genres 
            .Where(g => g.Id == id)
            .First();
    }

    /// <summary>
    /// Retrieves a list of aliases for a genre
    /// </summary>
    /// <param name="genre"> Specific genre </param>
    /// <returns> A collection of aliases corresponding to a specific genre </returns>
    public ICollection<GenreAlias> getAliases(Genre genre)
    {
        return _dbcontext.Genres
            .Where(g => g.Id == genre.Id)
            .Single()
            .Aliases;
    }

    /// <summary>
    /// Retrieves the parent(s) of a specific genre
    /// </summary>
    /// <param name="genre"> Specific genre </param>
    /// <returns> A collection of parents corresponding to a specified genre </returns>
    public ICollection<Genre> getParents(Genre genre)
    {
        return _dbcontext.Genres
            .Where(g => g.Id == genre.Id)
            .Single()
            .ParentGenres;
    }
    
    public ICollection<Genre> getRelated(Genre genre)
    {
        throw new NotImplementedException();
    }
}