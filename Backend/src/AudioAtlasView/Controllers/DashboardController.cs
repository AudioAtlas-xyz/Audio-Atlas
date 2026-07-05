using AudioAtlasApplication.DTOs.Dashboard;
using AudioAtlasApplication.Services.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AudioAtlasView.Controllers;

/// <summary>
/// Aggregated read-only metrics for the admin dashboard.
/// All queries are non-destructive; no state is mutated.
/// </summary>
[Route("api/admin/dashboard")]
[ApiController]
[Authorize(Roles = "Admin,Curator")]
public class DashboardController : ControllerBase
{
    private readonly ICatalogueQueryService _catalogue;
    private readonly IPipelineQueryService _pipeline;
    private readonly ICommunityQueryService _community;
    private readonly IDiscoveryQueryService _discovery;
    private readonly IConfiguration _config;

    public DashboardController(
        ICatalogueQueryService catalogue,
        IPipelineQueryService pipeline,
        ICommunityQueryService community,
        IDiscoveryQueryService discovery,
        IConfiguration config)
    {
        _catalogue = catalogue;
        _pipeline = pipeline;
        _community = community;
        _discovery = discovery;
        _config = config;
    }

    /// <summary>
    /// Returns aggregated admin metrics across five panels: Catalogue, Pipeline, Community, Discovery, and Cost.
    /// </summary>
    /// <remarks>
    /// **Filter parameters and scope**
    ///
    /// | Parameter | Affects |
    /// |-----------|---------|
    /// | continent | Catalogue (genre/country scope), Discovery (ContextContinent) |
    /// | region    | Catalogue (genre/country scope), Discovery (ContextRegion) |
    /// | country   | Catalogue (genre/country scope only) |
    /// | role      | Community › usersByRole display only |
    /// | from / to | Pipeline (ReviewedAt window), Discovery (OccurredAt window), Community newSignupsThisMonth is always the current calendar month |
    ///
    /// **Point-in-time values (ignore from/to):** queueDepth, oldestPendingAgeDays, approvedThisMonth,
    /// approvedLastMonth, rejectedThisMonth, rejectedLastMonth, sensitivityHolds.
    /// </remarks>
    /// <param name="filter">Optional filter parameters. All fields are nullable; omit any you don't need.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <response code="200">Dashboard data assembled successfully.</response>
    /// <response code="401">Missing or invalid JWT.</response>
    /// <response code="403">Authenticated user does not hold the Admin or Curator role.</response>
    [HttpGet]
    [ProducesResponseType(typeof(DashboardResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<DashboardResponse>> Get(
        [FromQuery] DashboardFilter filter,
        CancellationToken ct)
    {
        var earliestReviewAt = await _pipeline.GetEarliestReviewAtAsync(ct);
        var catalogue = await _catalogue.GetAsync(filter, ct);
        var pipeline = await _pipeline.GetAsync(filter, ct);
        var community = await _community.GetAsync(filter, ct);
        var discovery = await _discovery.GetAsync(filter, ct);

        var azureSpendRaw = _config["Admin:AzureMonthlySpend"];
        decimal? azureSpend = decimal.TryParse(azureSpendRaw, out var parsed) ? parsed : null;

        return Ok(new DashboardResponse
        {
            EarliestReviewAt = earliestReviewAt,
            Catalogue = catalogue,
            Pipeline = pipeline,
            Community = community,
            Discovery = discovery,
            Cost = new CostPanel
            {
                AzureMonthlySpend = azureSpend,
                Source = "manual"
            }
        });
    }
}
