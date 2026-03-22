using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

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
    /// Uses the development connection string to configure the context.
    /// This method is invoked by EF Core tools when performing tasks such as:
    /// - Adding migrations
    /// - Updating the database
    /// </summary>
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        return new AppDbContext(optionsBuilder.Options);
    }
}
