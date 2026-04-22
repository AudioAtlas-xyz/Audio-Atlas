using AudioAtlasDomain.Genres;
using AudioAtlasDomain.Geography;

namespace AudioAtlasApplication.Services;

public interface ICountryService
{
    public CountryDTO getCountry(string key);
    public CountryDTO getCountryById(Guid id);
    public CountryDTO getCountryByIsoCode(string isoCode);
}
