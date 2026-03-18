namespace AudioAtlasApplication.Repositories;

using AudioAtlasDomain.Geography;
using AudioAtlasDomain.Genres;
using Domain.Country;
using System.Collections.Generic;

public interface ICountryRepository
{
    public Country getCountryByID(Guid id);
    public Dictionary<Country, int> getGenreCounts();

    public ICollection<Genre> getGenres(Country country);

}