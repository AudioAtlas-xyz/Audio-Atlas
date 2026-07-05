namespace AudioAtlasApplication.DTOs.Dashboard;

/// <summary>
/// Shared query parameters for all dashboard panels.
/// Not every parameter affects every panel — see GET /api/admin/dashboard Swagger docs.
/// </summary>
public class DashboardFilter
{
    /// <summary>Limits Catalogue and Discovery metrics to genres/searches linked to this continent.</summary>
    public string? Continent { get; set; }

    /// <summary>Limits Catalogue and Discovery metrics to genres/searches linked to this region.</summary>
    public string? Region { get; set; }

    /// <summary>Limits Catalogue metrics to genres linked to this country name.</summary>
    public string? Country { get; set; }

    /// <summary>Limits Community usersByRole display. Values: Admin, Curator, Banned, Contributor.</summary>
    public string? Role { get; set; }

    /// <summary>UTC inclusive start for time-based metrics (Pipeline ReviewedAt, Discovery OccurredAt, Community signups).</summary>
    public DateTime? From { get; set; }

    /// <summary>UTC inclusive end for time-based metrics.</summary>
    public DateTime? To { get; set; }
}
