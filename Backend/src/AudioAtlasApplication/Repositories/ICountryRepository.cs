namespace AudioAtlasApplication.Repositories;

using AudioAtlasDomain.Geography;
using AudioAtlasDomain.Genres;

public interface ICountryRepository
{ //hey errbod'
    public Country getCountryByID(string id);
    public Dictionary<Country, int> getGenreCounts();

    public ICollection<Genre> getGenres(Country country);

}