namespace AudioAtlasApplication.Repositories;
using Domain.Country;
using System.Collections.Generic;

public interface ICountryRepository
{ //hey errbod'
    public Country getCountryByID(string id);
    public Dictionary<Country, int> getCountryWithGenreCount();

}