using AudioAtlasApplication.DTOs.Dashboard;

namespace AudioAtlasApplication.Services.Dashboard;

public interface IDiscoveryQueryService
{
    Task<DiscoveryPanel> GetAsync(DashboardFilter filter, CancellationToken ct = default);
}
