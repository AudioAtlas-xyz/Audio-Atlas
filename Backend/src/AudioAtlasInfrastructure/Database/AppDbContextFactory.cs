using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace AudioAtlasInfrastructure.Database;

/// <summary>
/// Provides a design-time factory for creating instances of AppDbContext.
///
/// This factory is used by EF Core tooling (e.g., migrations) to create
/// a DbContext instance when the application itself is not running.
///
/// It ensures that database operations such as migrations can be executed
/// without relying on the application's runtime configuration.
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    /// <summary>
    /// Creates a new instance of AppDbContext for design-time operations.
    ///
    /// Uses the startup project's configuration files and environment variables
    /// to construct the same SQL Server provider options used at runtime.
    /// </summary>
    public AppDbContext CreateDbContext(string[] args)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "AudioAtlasView");

        if (!Directory.Exists(basePath))
            basePath = Directory.GetCurrentDirectory();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new AppDbContext(optionsBuilder.Options);
    }
}
