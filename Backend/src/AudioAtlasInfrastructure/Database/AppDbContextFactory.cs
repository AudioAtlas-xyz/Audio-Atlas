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
    /// Uses the startup project's configuration so EF tooling resolves the
    /// same SQL Server provider and connection string as the running app.
    /// </summary>
    public AppDbContext CreateDbContext(string[] args)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        var startupProjectPath = ResolveStartupProjectPath();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(startupProjectPath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                $"Connection string 'DefaultConnection' was not found in '{startupProjectPath}'.");

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new AppDbContext(optionsBuilder.Options);
    }

    private static string ResolveStartupProjectPath()
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var candidates = new[]
        {
            Path.Combine(currentDirectory, "..", "AudioAtlasView"),
            Path.Combine(currentDirectory, "src", "AudioAtlasView"),
            Path.Combine(currentDirectory, "AudioAtlasView")
        };

        foreach (var candidate in candidates)
        {
            var fullPath = Path.GetFullPath(candidate);
            if (File.Exists(Path.Combine(fullPath, "appsettings.json")))
            {
                return fullPath;
            }
        }

        throw new InvalidOperationException(
            $"Could not locate the startup project configuration from '{currentDirectory}'.");
    }
}
