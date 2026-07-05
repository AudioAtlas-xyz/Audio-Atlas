using AudioAtlasApplication.DTOs.Dashboard;

namespace AudioAtlasApplication.Services.Dashboard;

public interface ICommunityQueryService
{
    Task<CommunityPanel> GetAsync(DashboardFilter filter, CancellationToken ct = default);
}
