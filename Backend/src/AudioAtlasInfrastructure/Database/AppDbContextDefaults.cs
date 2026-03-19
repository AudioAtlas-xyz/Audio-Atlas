namespace AudioAtlasInfrastructure.Database;

/// <summary>
/// Provides default configuration values for the database context.
/// 
/// This class defines environment-specific defaults used during development,
/// such as connection strings for local databases.
/// 
/// These values are intended for convenience and should not be relied upon
/// in production environments.
/// </summary>
public static class AppDbContextDefaults
{
    /// <summary>
    /// Default connection string used for local development.
    /// 
    /// Points to a local SQL Server instance (LocalDB) and creates or uses
    /// the "AudioAtlasDev" database.
    /// 
    /// Intended for development and testing only.
    /// Should be overridden by environment-specific configuration
    /// (e.g., appsettings, environment variables) in production.
    /// </summary>
    public const string DevelopmentConnectionString =
        "Server=(localdb)\\MSSQLLocalDB;Database=AudioAtlasDev;Trusted_Connection=True;MultipleActiveResultSets=true";
}
