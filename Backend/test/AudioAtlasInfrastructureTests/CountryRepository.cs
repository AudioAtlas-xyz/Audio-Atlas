using AudioAtlasApplication.Repositories;

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
        
    }
    
}
