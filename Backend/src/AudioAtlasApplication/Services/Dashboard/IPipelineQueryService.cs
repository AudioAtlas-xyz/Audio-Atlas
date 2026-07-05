using AudioAtlasApplication.DTOs.Dashboard;

namespace AudioAtlasApplication.Services.Dashboard;

public interface IPipelineQueryService
{
    Task<PipelinePanel> GetAsync(DashboardFilter filter, CancellationToken ct = default);

    /// <summary>Earliest non-null ReviewedAt across all submissions. Null if no submission has been reviewed.</summary>
    Task<DateTime?> GetEarliestReviewAtAsync(CancellationToken ct = default);
}
