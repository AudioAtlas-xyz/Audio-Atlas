using AudioAtlasDomain.Genres;
using AudioAtlasDomain.Geography;

namespace AudioAtlasApplication.Services;

public interface ICountryService
{
    public CountryDTO getCountryById(Guid id);
}