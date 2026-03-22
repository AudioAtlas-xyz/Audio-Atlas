using AudioAtlasApplication.Repositories;
using AudioAtlasInfrastructure.Database;
using AudioAtlasInfrastructure.Repositories;
using AudioAtlasInfrastructure.Database.Seed;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AudioAtlasTestServices;


public class TestService : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly IServiceScope _serviceScope;
    private readonly ServiceProvider _serviceProvider;
    private readonly ILogger<DbInitializer> _logger;
    
    public AppDbContext _context { get; }
    public ICountryRepository  _countryRepository { get; }
    public IGenreRepository  _genreRepository { get; }

    public TestService()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        
        var services = new ServiceCollection();
        
        
        services.AddDbContext<AppDbContext>(o => o.UseSqlite(_connection));
        services.AddLogging();

        services.AddScoped<ICountryRepository, CountryRepository>();
        services.AddScoped<IGenreRepository, GenreRepository>();
        
        _serviceProvider = services.BuildServiceProvider();
        
        _serviceScope = _serviceProvider.CreateScope();
        
        _logger = _serviceScope.ServiceProvider.GetService<ILogger<DbInitializer>>();
        _context = _serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        _countryRepository = _serviceScope.ServiceProvider.GetRequiredService<ICountryRepository>();
        _genreRepository = _serviceScope.ServiceProvider.GetRequiredService<IGenreRepository>();
        
        _context.Database.EnsureCreated();
        
        DbInitializer.SeedDatabase(_context, _logger);
    }

    public void Dispose()
    {
        _serviceScope.Dispose();
        _serviceProvider.Dispose();
        _connection.Close();
        _connection.Dispose();
    }
    
}