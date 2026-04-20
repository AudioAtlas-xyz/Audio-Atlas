using AudioAtlasApplication.Repositories;
using AudioAtlasDomain.Geography;
using AudioAtlasDomain.Genres;
using Xunit;

namespace AudioAtlasInfrastructureTests;
using AudioAtlasTestServices;

public class CountryRepositoryTests : IClassFixture<TestService>
{
    private readonly ICountryRepository _countryRepository;
    private readonly TestService _testService;

    private static Country CreateCountry(string name, List<Genre>? genres = null)
    {
        return new Country
        {
            Name = name,
            Region = "Test Region",
            Continent = "Test Continent",
            Description = "Test Description",
            isoCode = $"ISO-{Guid.NewGuid():N}"[..10],
            Genres = genres ?? new List<Genre>()
        };
    }

    public CountryRepositoryTests(TestService testService)
    {
        _testService = testService;
        _countryRepository = testService._countryRepository;
    }
    
    
    [Fact]
    public void getCountryByID_Works ()
    {
        var sampleCountry = CreateCountry("Test");
        
        _testService._context.Countries.Add(sampleCountry);
        _testService._context.SaveChanges();
        
        var id = sampleCountry.Id;
        var country = _countryRepository.getCountryByID(id);
        
        Assert.NotNull(country);
        Assert.Equal(sampleCountry.Id, country.Id);
        Assert.Equal(sampleCountry.Name, country.Name);
    }
    
    

    [Fact]
    public void getAllGenres_Works()
    {
        int CountryCount = _testService._context.Countries.Count();
        
        var result = _countryRepository.getGenreCounts();
        
        Assert.Equal(CountryCount, result.Count);
    }

    [Fact]
    public void getGenreCountPerCountry_Works()
    {
        
        var sampleCountry = CreateCountry(
            "Test",
            new List<Genre>
            {
                new Genre { Name = "Genre1"},
                new Genre { Name = "Genre2"}
            });
        
        _testService._context.Countries.Add(sampleCountry);
        _testService._context.SaveChanges();
        
        var id = sampleCountry.Id;
        
        var sampleCount =  sampleCountry.Genres.Count(); // 2
        var result = _countryRepository.getGenreCounts();


        Assert.True(result.ContainsKey("Test"));
        Assert.Equal(sampleCount, result["Test"]);
    }

    [Fact]
    public void getGenres_works()
    {
        var sampleGenre1 = new Genre { Name = "genre1" };
        var sampleGenre2 = new Genre { Name = "genre2" };
        
        var sampleCountry = CreateCountry("Test", new List<Genre> { sampleGenre1, sampleGenre2 });
        
        _testService._context.Countries.Add(sampleCountry);
        _testService._context.SaveChanges();
        
        var id =  sampleCountry.Id;
        var sampleCount =  sampleCountry.Genres.Count(); // 2
        var result = _countryRepository.getGenres(id);
        
        Assert.NotNull(result);
        Assert.Equal(sampleCount, result.Count);
        Assert.Contains(result, g => g.Name == "genre1");
        Assert.Contains(result, g => g.Name == "genre2");
    }
    
    
}
