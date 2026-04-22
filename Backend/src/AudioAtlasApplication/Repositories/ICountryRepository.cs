namespace AudioAtlasApplication.Repositories;

using AudioAtlasDomain.Geography;
using AudioAtlasDomain.Genres;
using System.Collections.Generic;

public interface ICountryRepository
{
    public Country getCountryByID(Guid id);
    public Country getCountryByIsoCode(string isoCode);
    public Dictionary<string, int> getGenreCounts();
    public ICollection<Genre> getGenres(Guid id);
    public ICollection<Country> getAllCountries();

}
