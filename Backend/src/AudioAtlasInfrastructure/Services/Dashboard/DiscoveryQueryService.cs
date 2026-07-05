using AudioAtlasApplication.DTOs.Dashboard;
using AudioAtlasApplication.Services.Dashboard;
using AudioAtlasInfrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace AudioAtlasInfrastructure.Services.Dashboard;

public class DiscoveryQueryService : IDiscoveryQueryService
{
    private readonly AppDbContext _db;

    public DiscoveryQueryService(AppDbContext db) => _db = db;

    public async Task<DiscoveryPanel> GetAsync(DashboardFilter filter, CancellationToken ct = default)
    {
        var baseQuery = _db.SearchQueries
            .Where(sq => filter.From == null || sq.OccurredAt >= filter.From)
            .Where(sq => filter.To == null || sq.OccurredAt <= filter.To)
            .Where(sq => filter.Continent == null || sq.ContextContinent == filter.Continent)
            .Where(sq => filter.Region == null || sq.ContextRegion == filter.Region);

        var zeroResultSearches = await baseQuery
            .Where(sq => sq.ResultCount == 0)
            .GroupBy(sq => sq.NormalizedTerm)
            .Select(g => new SearchTermDto { Term = g.Key, Frequency = g.Count() })
            .OrderByDescending(x => x.Frequency)
            .Take(20)
            .ToListAsync(ct);

        var topSearches = await baseQuery
            .GroupBy(sq => sq.NormalizedTerm)
            .Select(g => new SearchTermDto { Term = g.Key, Frequency = g.Count() })
            .OrderByDescending(x => x.Frequency)
            .Take(20)
            .ToListAsync(ct);

        return new DiscoveryPanel
        {
            ZeroResultSearches = zeroResultSearches,
            TopSearches = topSearches
        };
    }
}
