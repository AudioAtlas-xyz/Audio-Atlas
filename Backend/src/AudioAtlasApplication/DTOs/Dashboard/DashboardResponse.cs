namespace AudioAtlasApplication.DTOs.Dashboard;

public class DashboardResponse
{
    /// <summary>
    /// Earliest ReviewedAt timestamp found in the Submissions table.
    /// Pipeline latency metrics exclude rows with null ReviewedAt, so this
    /// value shows the UI the "data since" boundary for review-based panels.
    /// Null if no submission has been reviewed yet.
    /// </summary>
    public DateTime? EarliestReviewAt { get; set; }

    public CataloguePanel Catalogue { get; set; } = new();
    public PipelinePanel Pipeline { get; set; } = new();
    public CommunityPanel Community { get; set; } = new();
    public DiscoveryPanel Discovery { get; set; } = new();
    public CostPanel Cost { get; set; } = new();
}

// ── Shared primitives ──────────────────────────────────────────────────────

public class LabeledCountDto
{
    public string Label { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class LabeledShareDto
{
    public string Label { get; set; } = string.Empty;
    public int Count { get; set; }
    public double SharePercent { get; set; }
}

// ── Catalogue ─────────────────────────────────────────────────────────────

public class CataloguePanel
{
    /// <summary>Affected by continent/region/country filter.</summary>
    public int TotalGenres { get; set; }

    public List<LabeledCountDto> GenresByContinent { get; set; } = new();
    public List<LabeledCountDto> GenresByRegion { get; set; } = new();

    /// <summary>
    /// Content gate based on whether the genre has a non-empty Description.
    /// NOTE: The schema has no originsNote field; Description is used as proxy.
    /// </summary>
    public ContentGateDto ContentGate { get; set; } = new();

    public CountryCoverageDto CountryCoverage { get; set; } = new();
    public List<LabeledShareDto> GeographicBalance { get; set; } = new();
    public DataCompletenessDto DataCompleteness { get; set; } = new();

    /// <summary>Total rows in the GenreCountry join table.</summary>
    public int GenreCountryLinkCount { get; set; }
}

public class ContentGateDto
{
    public int Ready { get; set; }
    public int NotReady { get; set; }
}

public class CountryCoverageDto
{
    public int WithGenres { get; set; }
    public int Total { get; set; }
    public List<string> GapList { get; set; } = new();
}

public class DataCompletenessDto
{
    /// <summary>
    /// Genres with no linked countries. Always catalogue-wide: a genre with no
    /// countries can never satisfy a continent/region/country filter, so scoping
    /// this to the geography filter would report zero whenever one is applied.
    /// </summary>
    public int OrphanGenres { get; set; }

    /// <summary>Genres with no source links.</summary>
    public int MissingSources { get; set; }

    /// <summary>Genres flagged sensitive but with no sensitive description.</summary>
    public int SensitiveMissingDescription { get; set; }

    /// <summary>Genres with null PlaylistLink.</summary>
    public int MissingMedia { get; set; }
}

// ── Pipeline ──────────────────────────────────────────────────────────────

public class PipelinePanel
{
    /// <summary>Point-in-time count of Pending submissions. Ignores from/to.</summary>
    public int QueueDepth { get; set; }

    /// <summary>Days since the oldest Pending submission was submitted. Null if queue is empty.</summary>
    public double? OldestPendingAgeDays { get; set; }

    /// <summary>Approved in the current calendar month, keyed on ReviewedAt. Ignores from/to.</summary>
    public int ApprovedThisMonth { get; set; }

    /// <summary>Approved in the previous calendar month, keyed on ReviewedAt. Ignores from/to.</summary>
    public int ApprovedLastMonth { get; set; }

    /// <summary>Rejected in the current calendar month, keyed on ReviewedAt. Ignores from/to.</summary>
    public int RejectedThisMonth { get; set; }

    /// <summary>Rejected in the previous calendar month, keyed on ReviewedAt. Ignores from/to.</summary>
    public int RejectedLastMonth { get; set; }

    /// <summary>Approved / (Approved + Rejected) over the from/to window (all-time if unset).</summary>
    public double? ApprovalRate { get; set; }

    /// <summary>
    /// Median hours from SubmittedAt to ReviewedAt, excluding rows with null ReviewedAt.
    /// Respects from/to filter on ReviewedAt. Null if no reviewed submissions exist.
    /// </summary>
    public double? MedianTimeToReviewHours { get; set; }

    /// <summary>Decision count per reviewer over the from/to window. Respects from/to.</summary>
    public List<CuratorWorkloadDto> CuratorWorkload { get; set; } = new();

    /// <summary>Rejection count per RejectionReasonCode over the from/to window.</summary>
    public List<LabeledCountDto> RejectionBreakdown { get; set; } = new();

    /// <summary>Point-in-time count of OnHoldSensitivity submissions. Ignores from/to.</summary>
    public int SensitivityHolds { get; set; }
}

public class CuratorWorkloadDto
{
    public string ReviewerId { get; set; } = string.Empty;
    public string? ReviewerUsername { get; set; }
    public int Decisions { get; set; }
}

// ── Community ─────────────────────────────────────────────────────────────

public class CommunityPanel
{
    /// <summary>User count per role. Contributor = users with no assigned role.</summary>
    public List<LabeledCountDto> UsersByRole { get; set; } = new();

    /// <summary>Users whose AcceptedPrivacyPolicyAtUtc falls within the current calendar month.</summary>
    public int NewSignupsThisMonth { get; set; }

    /// <summary>Distinct submitters with a submission in the last 30 days.</summary>
    public int ActiveContributors { get; set; }

    public ContributorRetentionDto ContributorRetention { get; set; } = new();

    /// <summary>Top 10 contributors by total submission count.</summary>
    public List<ContributorSummaryDto> TopContributors { get; set; } = new();
}

public class ContributorRetentionDto
{
    public int Repeat { get; set; }
    public int OneTime { get; set; }
}

public class ContributorSummaryDto
{
    public string AccountId { get; set; } = string.Empty;
    public string? Username { get; set; }
    public int SubmissionCount { get; set; }
}

// ── Discovery ─────────────────────────────────────────────────────────────

public class DiscoveryPanel
{
    /// <summary>
    /// Top 20 zero-result searches by frequency.
    /// Respects from/to (OccurredAt) and continent/region (ContextContinent/ContextRegion).
    /// Note: topViewedGenres is omitted — no view tracking exists yet.
    /// </summary>
    public List<SearchTermDto> ZeroResultSearches { get; set; } = new();

    /// <summary>Top 20 searches by frequency, including those with results.</summary>
    public List<SearchTermDto> TopSearches { get; set; } = new();
}

public class SearchTermDto
{
    public string Term { get; set; } = string.Empty;
    public int Frequency { get; set; }
}

// ── Cost ──────────────────────────────────────────────────────────────────

public class CostPanel
{
    /// <summary>
    /// Read from config key Admin:AzureMonthlySpend. Null if not configured.
    /// This is a manually maintained figure — not retrieved from Azure Monitor.
    /// </summary>
    public decimal? AzureMonthlySpend { get; set; }

    public string Source { get; set; } = "manual";
}
