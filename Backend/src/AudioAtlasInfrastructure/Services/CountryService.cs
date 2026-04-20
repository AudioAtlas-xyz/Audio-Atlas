using AudioAtlasApplication.Repositories;
using AudioAtlasApplication.Services;
using AudioAtlasDomain.Genres;
using AudioAtlasDomain.Geography;

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
            Region = country.Region,
            Continent = country.Continent,
            IsoCode = country.isoCode,
            Genres = country.Genres.Select(genre => new GenreDTO
            {
                Id = genre.Id,
                Name = genre.Name,
                Summary = genre.Summary
            }).ToList()
        };
    }
}
