using AudioAtlasApplication.Repositories;
using AudioAtlasDomain.Geography;
using AudioAtlasInfrastructure.Database;
using AudioAtlasDomain.Genres;
using Microsoft.EntityFrameworkCore;

namespace AudioAtlasInfrastructure.Repositories;

public class CountryRepository : ICountryRepository
{
    readonly AppDbContext _dbcontext;
    
    /// <summary>
    /// Initialises a new instance of CountryRepository class
    /// </summary>
    /// <param name="appDbContext"></param>
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
        return _dbcontext.Countries
            .Where(c => c.Id == id)
            .Include(c => c.Genres)
            .ThenInclude(g => g.Author)
            .Single();
    }

    public async Task<ICollection<Country>> getCountriesByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken = default)
    {
        if (ids.Count == 0)
        {
            return Array.Empty<Country>();
        }

        return await _dbcontext.Countries
            .Where(country => ids.Contains(country.Id))
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves a country from the database based on ISO 3166-1 alpha-3 code.
    /// </summary>
    /// <param name="isoCode"> ISO 3166-1 alpha-3 country code </param>
    /// <returns> The country corresponding to the ISO code </returns>
    public Country getCountryByIsoCode(string isoCode)
    {
        string normalizedIsoCode = isoCode.Trim().ToUpperInvariant();

        return _dbcontext.Countries
            .Where(c => c.isoCode.ToUpper() == normalizedIsoCode)
            .Include(c => c.Genres)
            .ThenInclude(g => g.Author)
            .Single();
    }

    /// <summary>
    /// Retrieves all countries and their corresponding genre count from the database
    /// </summary>
    /// <returns> A dictionary of countries mapping to corresponding genre count </returns>
    public Dictionary<string,int> getGenreCounts()
    {
        return _dbcontext.Countries
            .Include(c => c.Genres)
            .ToDictionary(c => c.isoCode.ToUpperInvariant(), c => c.Genres.Count);
        
    }

    /// <summary>
    /// Retrieves a list of genres from a given country from the database
    /// </summary>
    /// <param name="country"> A specific country </param>
    /// <returns> A list of genres to the corresponding country </returns>
    public ICollection<Genre> getGenres(Guid id)
    {
        return _dbcontext.Countries
            .Where(c => c.Id == id)
            .Include(c => c.Genres)
            .Single()
            .Genres;
    }

    /// <summary>
    /// Retrieves a list of all countries in the database
    /// </summary>
    /// <returns> A list of all countries </returns>
    public ICollection<Country> getAllCountries()
    {
        return _dbcontext.Countries.ToList();
    }
    
}
