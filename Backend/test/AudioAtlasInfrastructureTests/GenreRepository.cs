
namespace AudioAtlasInfrastructureTests;

using AudioAtlasApplication.Repositories;
using AudioAtlasDomain.Genres;
using Xunit;
using AudioAtlasTestServices;


public class GenreRepository : IClassFixture<TestService>
{
    private readonly IGenreRepository _genreRepository;
    private readonly TestService _testService;

    public GenreRepository(TestService testService)
    {
        _testService = testService;
        _genreRepository = testService._genreRepository;
    }

    [Fact]
    public void getDescription_Works()
    {
        Genre sampleGenre = new Genre
        {
            Name = "Genre",
            Description = "Sample Genre"
        };
        
        _testService._context.Genres.Add(sampleGenre);
        _testService._context.SaveChanges();
        
        var result = _genreRepository.getDescription(sampleGenre.Id);

        Assert.NotNull(result);
        Assert.Equal("Sample Genre", result);
    }

    [Fact]
    public void getName_Works()
    {
        Genre sampleGenre = new Genre
        {
            Name = "Hiphop"
        };
        
        _testService._context.Genres.Add(sampleGenre);
        _testService._context.SaveChanges();

        var result = _genreRepository.getName(sampleGenre.Id);
        
        Assert.NotNull(result);
        Assert.Equal("Hiphop", result);
    }

    [Fact]
    public void getGenreByID_Works()
    {
        Genre sampleGenre = new Genre
        {
            Name = "Test",
        };
        
        _testService._context.Genres.Add(sampleGenre);
        _testService._context.SaveChanges();
        
        var id = sampleGenre.Id;
        var genre = _genreRepository.getGenre(id);
        
        Assert.NotNull(genre);
        Assert.Equal(sampleGenre.Id, genre.Id);
        Assert.Equal(sampleGenre.Name, genre.Name);
    }
    
    [Fact]
    public void getAliases_Works()
    {
        Genre sample = new Genre{Name = "sampleGenre"};
        sample.Aliases.Add(new GenreAlias{Alias = "sampleAlias"});
        _testService._context.Genres.Add(sample);
        _testService._context.SaveChanges();
        
        var expectedList = sample.Aliases;
        
        //Assert.Equal(expectedList, _genreRepository.getAliases(sample.Id));
        Assert.Contains(expectedList, a => a.Alias == "sampleAlias");
        Assert.Contains(_genreRepository.getAliases(sample.Id), a => a.Alias == "sampleAlias");
    }
    
    [Fact]
    public void getParents_Works()
    {
        var parent1 = new Genre { Name = "parent1" };
        var parent2 = new Genre { Name = "parent2" };

        var sample = new Genre { Name = "sampleGenre" };
        
        sample.ParentGenres.Add(parent1);
        sample.ParentGenres.Add(parent2);
        parent1.SubGenres.Add(sample);
        parent2.SubGenres.Add(sample);

        _testService._context.Genres.AddRange(parent1, parent2);
        _testService._context.Genres.Add(sample);
        _testService._context.SaveChanges();

        var id = sample.Id;
        var result = _genreRepository.getParents(id);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, g => g.Name == "parent1");
        Assert.Contains(result, g => g.Name == "parent2");
    }
    
    [Fact]
    public void getSubGenres_Works()
    {
        var parent = new Genre
        {
            Name = "parent"
        };
        var child1 = new Genre
        {
            Name = "child1"
        };
        var child2 = new Genre
        {
            Name = "child2"
        };

        parent.SubGenres.Add(child1);
        parent.SubGenres.Add(child2);
        child1.ParentGenres.Add(parent);
        child2.ParentGenres.Add(parent);
        
        _testService._context.Genres.AddRange(parent, child1, child2);
        _testService._context.SaveChanges();

        var result = _genreRepository.getSubGenres(parent.Id);

        Assert.NotNull(result);
        Assert.Contains(result, g => g.Name == "child1");
        Assert.Contains(result, g => g.Name == "child2");
        Assert.True(parent.SubGenres.Count == 2);
    }
    
    [Fact]
    public void getAllGenres_Works()
    {
        int expected = _testService._context.Genres.Count();
        int actual =  _genreRepository.getAllGenres().Count();
        
        Assert.Equal(actual, expected);
    }

}
