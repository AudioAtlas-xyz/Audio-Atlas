using AudioAtlasApplication.DTOs.Dashboard;

namespace AudioAtlasApplication.Services.Dashboard;

public interface ICatalogueQueryService
{
    Task<CataloguePanel> GetAsync(DashboardFilter filter, CancellationToken ct = default);
}
