using AudioAtlasInfrastructure.Repositories
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
    

namespace AudioAtlasTestService;

public class TestServices : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly IserviceScope _serviceScope;
    private readonly ServiceProvider _serviceProvider;
    
    public AppDbContext _context { get; }
    public ICountryRepository  _countryRepository { get; }
    public IGenreRepository  _genreRepository { get; }

    public TestServices()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var services = new ServiceCollection();
        
        services.AddDbContext<AppDbContext>(o => o.UseSqlite(_connection));

        services.AddSoped<ICountryRepository, CountryRepository>();
        services.AddSoped<IGenreRepository, GenreRepository>();
        
        _serviceProvider = services.BuildServiceProvider();
        _serviceScope = _serviceProvider.CreateScope();

        context = _serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.EnsureCreated();
        Dbinitializer.SeedDatabase(_context);
        
        
    }
    
}