using AudioAtlasApplication.DTOs.Dashboard;
using AudioAtlasApplication.Services.Dashboard;
using AudioAtlasDomain.Genres;
using AudioAtlasInfrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace AudioAtlasInfrastructure.Services.Dashboard;

public class CatalogueQueryService : ICatalogueQueryService
{
    private readonly AppDbContext _db;

    public CatalogueQueryService(AppDbContext db) => _db = db;

    public async Task<CataloguePanel> GetAsync(DashboardFilter filter, CancellationToken ct = default)
    {
        var filtered = FilteredGenres(filter);

        var total = await filtered.CountAsync(ct);

        var genresByContinent = await filtered
            .SelectMany(g => g.Countries.Select(c => new { g.Id, c.Continent }))
            .GroupBy(x => x.Continent)
            .Select(g => new LabeledCountDto { Label = g.Key, Count = g.Select(x => x.Id).Distinct().Count() })
            .OrderByDescending(x => x.Count)
            .ToListAsync(ct);

        var genresByRegion = await filtered
            .SelectMany(g => g.Countries.Select(c => new { g.Id, c.Region }))
            .GroupBy(x => x.Region)
            .Select(g => new LabeledCountDto { Label = g.Key, Count = g.Select(x => x.Id).Distinct().Count() })
            .OrderByDescending(x => x.Count)
            .ToListAsync(ct);

        var ready = await filtered.CountAsync(g => !string.IsNullOrEmpty(g.Description), ct);

        var geographicBalance = total == 0
            ? new List<LabeledShareDto>()
            : genresByContinent.Select(x => new LabeledShareDto
            {
                Label = x.Label,
                Count = x.Count,
                SharePercent = Math.Round((double)x.Count / total * 100, 1)
            }).ToList();

        // Country coverage scoped to the filter (e.g. only African countries when continent=Africa)
        var countriesInScope = _db.Countries
            .Where(c => filter.Continent == null || c.Continent == filter.Continent)
            .Where(c => filter.Region == null || c.Region == filter.Region)
            .Where(c => filter.Country == null || c.Name == filter.Country);

        var totalCountries = await countriesInScope.CountAsync(ct);
        var withGenres = await countriesInScope.CountAsync(c => c.Genres.Any(), ct);
        var gapList = await countriesInScope
            .Where(c => !c.Genres.Any())
            .Select(c => c.Name)
            .OrderBy(n => n)
            .Take(50)
            .ToListAsync(ct);

        var orphanGenres = await filtered.CountAsync(g => !g.Countries.Any(), ct);
        var missingDesc = await filtered.CountAsync(g => string.IsNullOrEmpty(g.Description), ct);
        var missingMedia = await filtered.CountAsync(g => g.PlaylistLink == null, ct);

        var linkCount = await filtered.SelectMany(g => g.Countries).CountAsync(ct);

        return new CataloguePanel
        {
            TotalGenres = total,
            GenresByContinent = genresByContinent,
            GenresByRegion = genresByRegion,
            ContentGate = new ContentGateDto { Ready = ready, NotReady = total - ready },
            GeographicBalance = geographicBalance,
            CountryCoverage = new CountryCoverageDto
            {
                WithGenres = withGenres,
                Total = totalCountries,
                GapList = gapList
            },
            DataCompleteness = new DataCompletenessDto
            {
                OrphanGenres = orphanGenres,
                MissingOriginsNote = missingDesc,
                MissingMedia = missingMedia
            },
            GenreCountryLinkCount = linkCount
        };
    }

    private IQueryable<Genre> FilteredGenres(DashboardFilter f) =>
        _db.Genres
            .Where(g => f.Continent == null || g.Countries.Any(c => c.Continent == f.Continent))
            .Where(g => f.Region == null || g.Countries.Any(c => c.Region == f.Region))
            .Where(g => f.Country == null || g.Countries.Any(c => c.Name == f.Country));
}
