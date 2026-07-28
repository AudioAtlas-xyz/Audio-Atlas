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
        // Wait for Azure SQL to stabilize after cold start before seeding.
        await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);

        try
        {
            using IServiceScope scope = _scopeFactory.CreateScope();
            AppDbContext ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            ILogger<DbInitializer> seedLogger = scope.ServiceProvider.GetRequiredService<ILogger<DbInitializer>>();
            await DbInitializer.SeedAdditionalDataAsync(ctx, seedLogger);
        }
        catch (Exception ex)
        {
            // Deliberately swallowed so a seeding fault cannot take the API down —
            // but that means this log line is the ONLY signal. "SeederFailed" is a
            // stable marker to build an alert rule on; this failure previously went
            // unnoticed for days while every genre batch silently lost its
            // relations. See LogDataQualityAsync for the companion signal.
            _logger.LogError(ex, "SeederFailed: supplemental seeder encountered an unhandled error. Genre relations may be missing.");
        }
    }
}
