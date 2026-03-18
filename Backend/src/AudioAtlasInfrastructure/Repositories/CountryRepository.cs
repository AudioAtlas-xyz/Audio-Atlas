using AudioAtlasApplication.Repositories;
using AudioAtlasDomain.Geography;
using AudioAtlasInfrastructure.Database;
using AudioAtlasDomain.Genres;
using Microsoft.EntityFrameworkCore;

namespace AudioAtlasInfrastructure.Repositories;

public class CountryRepository : ICountryRepository
{
    readonly AppDbContext _dbcontext;
    public CountryRepository(AppDbContext appDbContext)
    {
        _dbcontext = appDbContext;
    }
    
    
    /// <summary>
    /// Retrieves a country from the database based on id
    /// </summary>
    /// <param name="id"> Unique identifier for a specific country </param>
    /// <returns> The country corresponding to the ID or null </returns>
    public Country getCountryByID(Guid id)
    {
        return _dbcontext.Countries.Find(id);
    }

    /// <summary>
    /// Retrieves all countries and their corresponding genre count from the database
    /// </summary>
    /// <returns> A dictionary of countries mapping to corresponding genre count </returns>
    public Dictionary<string,int> getGenreCounts()
    {
        return _dbcontext.Countries
            .Include(c => c.Genres)
            .ToDictionary(c => c.Name, c => c.Genres.Count);
        
    }

    /// <summary>
    /// Retrieves a list of genres from a given country from the database
    /// </summary>
    /// <param name="country"> A specific country </param>
    /// <returns> A list of genres to the corresponding country </returns>
    public ICollection<Genre> getGenres(Guid id)
    {
        Country country = getCountryByID(id);
        return _dbcontext.Countries
            .Include(c =>  c.Genres)
            .Where(c => c.Name == country.Name)
            .First()
            .Genres;
    }

    public ICollection<Country> getAllCountries()
    {
        return _dbcontext.Countries.ToList();
    }
    
}