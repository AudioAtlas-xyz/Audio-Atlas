using AudioAtlasApplication.Repositories;
using AudioAtlasApplication.Services;
using AudioAtlasDomain.Genres;
using AudioAtlasDomain.Geography;
using AudioAtlasInfrastructure.Repositories;

namespace AudioAtlasInfrastructure.Services;

public class CountryService : ICountryService
{
    private readonly ICountryRepository _countryRepository;
    
    public CountryService(ICountryRepository countryRepository)
    {
        _countryRepository = countryRepository;
    }
    
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