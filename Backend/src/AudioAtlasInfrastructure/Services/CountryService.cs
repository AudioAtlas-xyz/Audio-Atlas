using AudioAtlas.Domain.Geography;
using AudioAtlasApplication.Repositories;
using AudioAtlasApplication.Services;
using AudioAtlasDomain.Genres;
using AudioAtlasDomain.Geography;
using AudioAtlasDomain.Users;
using Microsoft.Extensions.Logging;

namespace AudioAtlasInfrastructure.Services;

public class CountryService : ICountryService
{
    private readonly ICountryRepository _countryRepository;
    private readonly IGenreRepository _genreRepository;
    private readonly ILogger<CountryService> _logger;

    public CountryService(ICountryRepository countryRepository, IGenreRepository genreRepository, ILogger<CountryService> logger)
    {
        _countryRepository = countryRepository;
        _genreRepository = genreRepository;
        _logger = logger;
    }

    public CountryDTO getCountry(string key)
    {
        if (Guid.TryParse(key, out Guid id))
        {
            return getCountryById(id);
        }

        return getCountryByIsoCode(key);
    }

    public CountryDTO getCountryById(Guid id)
    {
        Country country = _countryRepository.getCountryByID(id);

        return toCountryDTO(country);
    }

    public CountryDTO getCountryByIsoCode(string isoCode)
    {
        Country country = _countryRepository.getCountryByIsoCode(isoCode);

        return toCountryDTO(country);
    }

    private CountryDTO toCountryDTO(Country country)
    {

        List<GenreDTO> genreDTOs = new List<GenreDTO>();
        List<ContributorSummaryDTO> contributors = new List<ContributorSummaryDTO>();
        HashSet<ApplicationUser> addedUsers = new HashSet<ApplicationUser>();

        foreach (Genre genre in country.Genres)
        {
            genreDTOs.Add(new GenreDTO
            {
                Id = genre.Id,
                Name = genre.Name,
                Description = genre.Description
            });


            ApplicationUser? user = genre.Author;



            if (user != null && !addedUsers.Contains(user))
            {

                int genreCount = _genreRepository.getGenresByAuthorId(user.Id).Count;
                contributors.Add(new ContributorSummaryDTO
                {
                    id = user.Id.ToString(),
                    username = user.UserName,
                    genreCount = genreCount
                });

                addedUsers.Add(user);
            }

        }

        return new CountryDTO
        {
            Id = country.Id,
            Name = country.Name,
            Description = country.Description,
            Region = country.Region,
            Continent = country.Continent,
            IsoCode = country.isoCode,
            Genres = genreDTOs,
            Contributors = contributors
        };
    }
}
