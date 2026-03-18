using AudioAtlasApplication.Repositories;
using AudioAtlasDomain.Geography;
using Xunit;

namespace AudioAtlasInfrastructureTests;
using AudioAtlasTestServices;

public class CountryRepositoryTests : IClassFixture<TestService>
{
    private readonly ICountryRepository _countryRepository;
    private readonly TestService _testService;
    public CountryRepositoryTests(TestService testService)
    {
        _testService = testService;
        _countryRepository = testService._countryRepository;
    }
    
    
    [Fact]
    public void getCountryByID_Works ()
    {
        var sampleCountry = new Country
        {
            Name = "Test"
        };
        
        _testService._context.Countries.Add(sampleCountry);
        _testService._context.SaveChanges();
        
        var id = sampleCountry.Id;
        
        var country = _countryRepository.getCountryByID(id);
        Assert.Equal(sampleCountry.Id, country.Id);
        Assert.Equal(sampleCountry.Name, country.Name);
    }
    
    

    [Fact]
    public void getAllGenres_Works()
    {
        int CountryCount = 106;
        
        var result = _countryRepository.getGenreCounts();
        
        Assert.Equal(CountryCount, result.Count);
    }

    [Fact]
    public void getGenreCountPerCountry_Works()
    {
        int GhanaCount = 12;
        //Country Ghana = _countryRepository.getCountryByID(); //??? :D
        
       // var result = _countryRepository.getGenreCounts()[Ghana]
    }
    
}
