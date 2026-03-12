using AudioAtlasApplication.Repositories;
using Domain.Country;

namespace AudioAtlasInfrastructure.Repositories;

public class CountryRepository : ICountryRepository
{
    public Country getCountryByID(string id)
    {
        throw new NotImplementedException();
    }

    public Dictionary<Country, int> getCountryWithGenreCount()
    {
        throw new NotImplementedException();
    }
}