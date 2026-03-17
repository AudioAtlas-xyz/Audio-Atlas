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

        services.AddScoped<ICountryRepository, CountryRepository>();
        services.AddScoped<IGenreRepository, GenreRepository>();
        services.AddScoped<ILogger<DbInitializer>, Logger<DbInitializer>>();
        
        _serviceProvider = services.BuildServiceProvider();
        _serviceScope = _serviceProvider.CreateScope();

        _context = _serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
        _context.Database.EnsureCreated();
        DbInitializer.SeedDatabase(_context, _logger);
        
        _countryRepository = _serviceProvider.GetRequiredService<ICountryRepository>();
        _genreRepository = _serviceProvider.GetRequiredService<IGenreRepository>();
        
    }

    public void Dispose()
    {
        _serviceScope.Dispose();
        _serviceProvider.Dispose();
        _connection.Close();
        _connection.Dispose();
    }
    
}