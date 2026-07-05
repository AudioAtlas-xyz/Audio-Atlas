using AudioAtlasApplication.DTOs.Dashboard;
using AudioAtlasApplication.Services.Dashboard;
using AudioAtlasDomain.Enums;
using AudioAtlasDomain.Submissions;
using AudioAtlasInfrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace AudioAtlasInfrastructure.Services.Dashboard;

public class PipelineQueryService : IPipelineQueryService
{
    private readonly AppDbContext _db;

    public PipelineQueryService(AppDbContext db) => _db = db;

    public async Task<DateTime?> GetEarliestReviewAtAsync(CancellationToken ct = default) =>
        await _db.Submissions
            .Where(s => s.ReviewedAt != null)
            .MinAsync(s => (DateTime?)s.ReviewedAt, ct);

    public async Task<PipelinePanel> GetAsync(DashboardFilter filter, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var startOfThisMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var startOfLastMonth = startOfThisMonth.AddMonths(-1);

        var queueDepth = await _db.Submissions.CountAsync(s => s.Status == SubmissionStatus.Pending, ct);

        var oldestPendingDate = await _db.Submissions
            .Where(s => s.Status == SubmissionStatus.Pending)
            .MinAsync(s => (DateTime?)s.SubmittedAt, ct);
        double? oldestPendingAgeDays = oldestPendingDate.HasValue
            ? (now - oldestPendingDate.Value).TotalDays
            : null;

        var approvedThisMonth = await _db.Submissions.CountAsync(
            s => s.Status == SubmissionStatus.Approved
              && s.ReviewedAt >= startOfThisMonth
              && s.ReviewedAt < startOfThisMonth.AddMonths(1), ct);

        var approvedLastMonth = await _db.Submissions.CountAsync(
            s => s.Status == SubmissionStatus.Approved
              && s.ReviewedAt >= startOfLastMonth
              && s.ReviewedAt < startOfThisMonth, ct);

        var rejectedThisMonth = await _db.Submissions.CountAsync(
            s => s.Status == SubmissionStatus.Rejected
              && s.ReviewedAt >= startOfThisMonth
              && s.ReviewedAt < startOfThisMonth.AddMonths(1), ct);

        var rejectedLastMonth = await _db.Submissions.CountAsync(
            s => s.Status == SubmissionStatus.Rejected
              && s.ReviewedAt >= startOfLastMonth
              && s.ReviewedAt < startOfThisMonth, ct);

        var reviewedQuery = ReviewedQuery(filter);

        var approved = await reviewedQuery.CountAsync(s => s.Status == SubmissionStatus.Approved, ct);
        var rejected = await reviewedQuery.CountAsync(s => s.Status == SubmissionStatus.Rejected, ct);
        double? approvalRate = (approved + rejected) > 0
            ? Math.Round((double)approved / (approved + rejected) * 100, 1)
            : null;

        // Load timestamps into memory for median computation — no SQL median function needed
        var durations = await reviewedQuery
            .Select(s => new { s.SubmittedAt, ReviewedAt = s.ReviewedAt!.Value })
            .ToListAsync(ct);

        double? medianHours = null;
        if (durations.Count > 0)
        {
            var sorted = durations.Select(d => (d.ReviewedAt - d.SubmittedAt).TotalHours).OrderBy(h => h).ToList();
            int mid = sorted.Count / 2;
            medianHours = sorted.Count % 2 == 1
                ? sorted[mid]
                : (sorted[mid - 1] + sorted[mid]) / 2.0;
            medianHours = Math.Round(medianHours.Value, 2);
        }

        // Curator workload: group in DB, then look up usernames
        var workloadRaw = await reviewedQuery
            .Where(s => s.ReviewedById != null)
            .GroupBy(s => s.ReviewedById)
            .Select(g => new { ReviewedById = g.Key, Decisions = g.Count() })
            .ToListAsync(ct);

        List<CuratorWorkloadDto> curatorWorkload = new();
        if (workloadRaw.Count > 0)
        {
            var reviewerIds = workloadRaw.Select(w => w.ReviewedById!.Value).ToList();
            var usernameMap = await _db.Users
                .Where(u => reviewerIds.Contains(u.Id))
                .Select(u => new { u.Id, u.UserName })
                .ToDictionaryAsync(u => u.Id, u => u.UserName, ct);

            curatorWorkload = workloadRaw
                .Select(w => new CuratorWorkloadDto
                {
                    ReviewerId = w.ReviewedById!.Value.ToString(),
                    ReviewerUsername = usernameMap.GetValueOrDefault(w.ReviewedById!.Value),
                    Decisions = w.Decisions
                })
                .OrderByDescending(x => x.Decisions)
                .ToList();
        }

        var rejectionBreakdown = await reviewedQuery
            .Where(s => s.Status == SubmissionStatus.Rejected && s.RejectionReasonCode != null)
            .GroupBy(s => s.RejectionReasonCode!)
            .Select(g => new LabeledCountDto { Label = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToListAsync(ct);

        var sensitivityHolds = await _db.Submissions
            .CountAsync(s => s.Status == SubmissionStatus.OnHoldSensitivity, ct);

        return new PipelinePanel
        {
            QueueDepth = queueDepth,
            OldestPendingAgeDays = oldestPendingAgeDays,
            ApprovedThisMonth = approvedThisMonth,
            ApprovedLastMonth = approvedLastMonth,
            RejectedThisMonth = rejectedThisMonth,
            RejectedLastMonth = rejectedLastMonth,
            ApprovalRate = approvalRate,
            MedianTimeToReviewHours = medianHours,
            CuratorWorkload = curatorWorkload,
            RejectionBreakdown = rejectionBreakdown,
            SensitivityHolds = sensitivityHolds
        };
    }

    private IQueryable<Submission> ReviewedQuery(DashboardFilter f) =>
        _db.Submissions
            .Where(s => s.ReviewedAt != null)
            .Where(s => f.From == null || s.ReviewedAt >= f.From)
            .Where(s => f.To == null || s.ReviewedAt <= f.To);
}
