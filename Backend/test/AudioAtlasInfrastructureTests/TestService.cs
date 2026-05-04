using AudioAtlasApplication.Repositories;
using AudioAtlasDomain.Users;
using AudioAtlasInfrastructure.Database;
using AudioAtlasInfrastructure.Database.Seed;
using AudioAtlasInfrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AudioAtlasTestServices;


public class TestService : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly IServiceScope _serviceScope;
    private readonly ServiceProvider _serviceProvider;
    private readonly ILogger<DbInitializer> _logger;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    
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
        services
            .AddIdentityCore<ApplicationUser>()
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<AppDbContext>();

        services.AddScoped<ICountryRepository, CountryRepository>();
        services.AddScoped<IGenreRepository, GenreRepository>();
        
        _serviceProvider = services.BuildServiceProvider();
        
        _serviceScope = _serviceProvider.CreateScope();
        
        _logger = _serviceScope.ServiceProvider.GetRequiredService<ILogger<DbInitializer>>();
        _context = _serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
        _userManager = _serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        _roleManager = _serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        _countryRepository = _serviceScope.ServiceProvider.GetRequiredService<ICountryRepository>();
        _genreRepository = _serviceScope.ServiceProvider.GetRequiredService<IGenreRepository>();
        
        _context.Database.EnsureCreated();
        
        DbInitializer.SeedDatabase(_context, _userManager, _roleManager, _logger).GetAwaiter().GetResult();
    }

    public void Dispose()
    {
        _serviceScope.Dispose();
        _serviceProvider.Dispose();
        _connection.Close();
        _connection.Dispose();
    }
    
}
