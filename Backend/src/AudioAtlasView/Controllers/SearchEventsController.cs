using AudioAtlasApplication.DTOs;
using AudioAtlasApplication.Repositories;
using AudioAtlasDomain.Search;
using Microsoft.AspNetCore.Mvc;

namespace AudioAtlasView.Controllers;

/// <summary>
/// Accepts client-side search telemetry.
/// Public endpoint — no authentication required.
/// Stores query text and result count only; no PII (no IP, no user ID).
/// </summary>
[Route("api/search-events")]
[ApiController]
public class SearchEventsController : ControllerBase
{
    private const int MaxTermLength = 200;
    private const int MaxContextLength = 200;

    private readonly ISearchQueryRepository _repository;

    public SearchEventsController(ISearchQueryRepository repository)
    {
        _repository = repository;
    }

    [HttpPost]
    public async Task<IActionResult> Log(
        [FromBody] LogSearchQueryRequest request,
        CancellationToken cancellationToken)
    {
        var term = request.Term?.Trim() ?? string.Empty;

        if (string.IsNullOrEmpty(term) || term.Length > MaxTermLength)
            return NoContent();

        var query = new SearchQuery
        {
            Term = term,
            NormalizedTerm = term.ToLowerInvariant(),
            ResultCount = Math.Max(0, request.ResultCount),
            OccurredAt = DateTime.UtcNow,
            ContextRegion = truncate(request.ContextRegion?.Trim(), MaxContextLength),
            ContextContinent = truncate(request.ContextContinent?.Trim(), MaxContextLength)
        };

        try
        {
            await _repository.logAsync(query, cancellationToken);
        }
        catch
        {
            // Logging must never surface failures to the caller.
        }

        return NoContent();
    }

    private static string? truncate(string? value, int max) =>
        value is { Length: > 0 } ? (value.Length <= max ? value : value[..max]) : null;
}
