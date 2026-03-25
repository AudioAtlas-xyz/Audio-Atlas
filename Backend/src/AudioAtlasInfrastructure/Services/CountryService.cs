using AudioAtlasApplication.Repositories;
using AudioAtlasApplication.Services;
using AudioAtlasDomain.Genres;
using AudioAtlasDomain.Geography;
using AudioAtlasInfrastructure.Repositories;

namespace AudioAtlasInfrastructure.Services;

public class CountryService : ICountryService
{
    private readonly ICountryRepository _countryRepository;
    
    /// <summary>
    /// Initialises a new instance of CountryService class
    /// </summary>
    /// <param name="countryRepository"> Repository used for retrieving country related data in the database </param>
    public CountryService(ICountryRepository countryRepository)
    {
        _countryRepository = countryRepository;
    }
    
    /// <summary>
    /// Takes a country and converts it into a CountryDTO
    /// </summary>
    /// <param name="id"> The ID corresponding to a country </param>
    /// <returns> A CountryDTO based on a specific country </returns>
    public CountryDTO getCountryById(Guid id)
    {
        Country country = _countryRepository.getCountryByID(id);

        return new CountryDTO
        {
            Id = country.Id,
            Name = country.Name,
            Description = country.Description,
            Submissions = country.Submissions,

            // Make DTOs instead of Genre objects
            Genres = country.Genres.Select(g => new GenreDTO
            {
                Name = g.Name,
                Id = g.Id
            }).ToList(),
        };

    }
}