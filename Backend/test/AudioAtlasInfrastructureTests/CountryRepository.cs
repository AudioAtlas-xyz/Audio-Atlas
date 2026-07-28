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
        
        var isoKey = sampleCountry.isoCode.ToUpperInvariant();
        var sampleCount = sampleCountry.Genres.Count(); // 2
        var result = _countryRepository.getGenreCounts();

        Assert.True(result.ContainsKey(isoKey));
        Assert.Equal(sampleCount, result[isoKey]);
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

    [Fact]
    public void getGroupings_counts_distinct_genres_not_per_country_totals()
    {
        // The obvious implementation — summing each country's genre count — is
        // wrong: a genre spanning several countries in the same grouping would be
        // counted once per country, inflating every region that shares a genre.
        // The browse listing itself shows distinct genres, so the counts beside
        // the navigation links have to agree with it.
        string continent = $"Continent-{Guid.NewGuid():N}";
        string regionA = $"RegionA-{Guid.NewGuid():N}";
        string regionB = $"RegionB-{Guid.NewGuid():N}";

        var shared = new Genre { Name = $"Shared-{Guid.NewGuid():N}" };
        var localOnly = new Genre { Name = $"Local-{Guid.NewGuid():N}" };

        Country Build(string region, List<Genre> genres) => new()
        {
            Name = $"Country-{Guid.NewGuid():N}",
            Region = region,
            Continent = continent,
            Description = "Test Description",
            isoCode = $"ISO-{Guid.NewGuid():N}"[..10],
            Genres = genres
        };

        // Two countries in the same region both carry `shared`; one also carries
        // a second genre. A third country in a sibling region carries `shared`.
        _testService._context.Countries.Add(Build(regionA, [shared]));
        _testService._context.Countries.Add(Build(regionA, [shared, localOnly]));
        _testService._context.Countries.Add(Build(regionB, [shared]));
        _testService._context.SaveChanges();

        var grouping = _countryRepository.getGroupings().Single(g => g.Continent == continent);

        // Naive per-country summing would give 4 here, and 3 for region A.
        Assert.Equal(2, grouping.GenreCount);
        Assert.Equal(2, grouping.Regions.Single(r => r.Region == regionA).GenreCount);
        Assert.Equal(1, grouping.Regions.Single(r => r.Region == regionB).GenreCount);
        Assert.Equal(2, grouping.Regions.Count);
    }

    [Fact]
    public void getGroupings_keeps_regions_that_have_no_genres()
    {
        // Empty regions are kept and report zero rather than disappearing, so the
        // navigation can show them as empty instead of silently omitting them.
        string continent = $"Continent-{Guid.NewGuid():N}";
        string emptyRegion = $"Empty-{Guid.NewGuid():N}";

        _testService._context.Countries.Add(new Country
        {
            Name = $"Country-{Guid.NewGuid():N}",
            Region = emptyRegion,
            Continent = continent,
            Description = "Test Description",
            isoCode = $"ISO-{Guid.NewGuid():N}"[..10],
            Genres = []
        });
        _testService._context.SaveChanges();

        var grouping = _countryRepository.getGroupings().Single(g => g.Continent == continent);

        Assert.Equal(0, grouping.GenreCount);
        Assert.Equal(0, grouping.Regions.Single(r => r.Region == emptyRegion).GenreCount);
    }
}
