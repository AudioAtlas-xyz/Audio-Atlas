using AudioAtlasApplication.Repositories;
using AudioAtlasDomain.Genres;
using AudioAtlasInfrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

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
    public string getDescription(Guid id)
    {
        return _dbcontext.Genres
            .Where(g => g.Id == id)
            .Single()
            .Description;
    }

    /// <summary>
    /// Retrieves the name corresponding to the genre
    /// </summary>
    /// <param name="genre"> Specific genre </param>
    /// <returns> A string corresponding to the genres name </returns>
    public string getName(Guid id)
    {
        return _dbcontext.Genres
            .Where(g => g.Id == id)
            .Single()
            .Name;
    }

    /// <summary>
    /// Retrieves a genre from the database based on id
    /// </summary>
    /// <param name="id"> Unique identifier for a specific genre </param>
    /// <returns> The genre corresponding to the ID </returns>
    public Genre getGenre(Guid id)
    {
        return _dbcontext.Genres
            .Where(g => g.Id == id)
            .Single();

    }

    /// <summary>
    /// Retrieves a list of aliases for a genre
    /// </summary>
    /// <param name="genre"> Specific genre </param>
    /// <returns> A collection of aliases corresponding to a specific genre </returns>
    public ICollection<GenreAlias> getAliases(Guid id)
    {
        return _dbcontext.Genres
            .Include(g => g.Aliases)
            .Where(g => g.Id == id)
            .Single()
            .Aliases;
    }

    /// <summary>
    /// Retrieves the parent(s) of a specific genre
    /// </summary>
    /// <param name="genre"> Specific genre </param>
    /// <returns> A collection of parents corresponding to a specified genre </returns>
    public ICollection<Genre> getParents(Guid id)
    {
        return _dbcontext.Genres
            .Include(g => g.ParentGenres)
            .Where(g => g.Id == id)
            .Single()
            .ParentGenres;
    }
    
    /// <summary>
    /// Retrieves the subgenre(s) of a specific genre
    /// </summary>
    /// <param name="genre"> Specific genre </param>
    /// <returns> A collection of subgenres corresponding to a specified genre </returns>
    public ICollection<Genre> getSubGenres(Guid id)
    {
        return _dbcontext.Genres
            .Include(g => g.SubGenres)
            .Where(g => g.Id == id)
            .Single()
            .SubGenres;
    }
    
    /// <summary>
    /// Retrieves the similar genre(s) of a specific genre
    /// </summary>
    /// <param name="genre"> Specific genre </param>
    /// <returns> A collection of similar genres corresponding to a specified genre </returns>
    public ICollection<Genre> getSimilarGenres(Guid id)
    {
        return _dbcontext.Genres
            .Include(g => g.SimilarGenres)
            .Where(g => g.Id == id)
            .Single()
            .SimilarGenres;
    }

    /// <summary>
    /// Retrieves the related genre(s) to a specific genre
    /// </summary>
    /// <param name="genre"> Specific genre </param>
    /// <returns> A list with all related genres to a specific genre </returns>
    public ICollection<Genre> getRelated(Guid id)
    {
        return _dbcontext.Genres
            .Where(g => g.Id == id)
            .Single()
            .SubGenres.Union(getParents(id)).Union(getSimilarGenres(id))
            .ToList();
    }
    
    /// <summary>
    /// Retrieves all genres from the database
    /// </summary>
    /// <returns> A list with all genres </returns>
    public ICollection<Genre> getAllGenres()
    {
        return _dbcontext.Genres.ToList();
    }
        
}   