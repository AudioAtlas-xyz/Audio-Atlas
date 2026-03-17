namespace AudioAtlasApplication.Repositories;

using AudioAtlasDomain.Geography;
using AudioAtlasDomain.Genres;

public interface ICountryRepository
{
    public Country getCountryByID(Guid id);
    public Dictionary<Country, int> getGenreCounts();

    public ICollection<Genre> getGenres(Country country);

}