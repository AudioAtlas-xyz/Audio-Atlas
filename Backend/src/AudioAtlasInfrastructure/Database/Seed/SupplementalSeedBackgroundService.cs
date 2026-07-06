using AudioAtlasInfrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AudioAtlasInfrastructure.Database.Seed;

public sealed class SupplementalSeedBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<SupplementalSeedBackgroundService> _logger;

    public SupplementalSeedBackgroundService(IServiceScopeFactory scopeFactory, ILogger<SupplementalSeedBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            using IServiceScope scope = _scopeFactory.CreateScope();
            AppDbContext ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            ILogger<DbInitializer> seedLogger = scope.ServiceProvider.GetRequiredService<ILogger<DbInitializer>>();
            await DbInitializer.SeedAdditionalDataAsync(ctx, seedLogger);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Supplemental seeder encountered an unhandled error.");
        }
    }
}
