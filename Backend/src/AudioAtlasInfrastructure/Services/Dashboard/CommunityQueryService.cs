using AudioAtlasApplication.DTOs.Dashboard;
using AudioAtlasApplication.Services.Dashboard;
using AudioAtlasInfrastructure.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AudioAtlasInfrastructure.Services.Dashboard;

public class CommunityQueryService : ICommunityQueryService
{
    private readonly AppDbContext _db;

    public CommunityQueryService(AppDbContext db) => _db = db;

    public async Task<CommunityPanel> GetAsync(DashboardFilter filter, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var startOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        // ── UsersByRole ────────────────────────────────────────────────────────
        var rolesList = await _db.Roles
            .Select(r => new { r.Id, r.Name })
            .ToListAsync(ct);

        var userRolesFlat = await _db.UserRoles
            .Select(ur => new { ur.UserId, ur.RoleId })
            .ToListAsync(ct);

        var totalNonSystem = await _db.Users
            .CountAsync(u => !u.IsSystemUser && !u.IsDeletedPlaceholder, ct);

        var usersWithAnyRole = userRolesFlat.Select(ur => ur.UserId).Distinct().Count();
        int contributorCount = totalNonSystem - usersWithAnyRole;

        var roleIdToName = rolesList.ToDictionary(r => r.Id, r => r.Name ?? "Unknown");

        var roleNameToCount = userRolesFlat
            .GroupBy(ur => roleIdToName.GetValueOrDefault(ur.RoleId, "Unknown"))
            .ToDictionary(g => g.Key, g => g.Select(ur => ur.UserId).Distinct().Count());

        List<LabeledCountDto> usersByRole;

        if (filter.Role != null && filter.Role.Equals("Contributor", StringComparison.OrdinalIgnoreCase))
        {
            usersByRole = new List<LabeledCountDto>
            {
                new() { Label = "Contributor", Count = contributorCount }
            };
        }
        else if (filter.Role != null)
        {
            var count = roleNameToCount.GetValueOrDefault(filter.Role, 0);
            usersByRole = new List<LabeledCountDto>
            {
                new() { Label = filter.Role, Count = count }
            };
        }
        else
        {
            usersByRole = roleNameToCount
                .Select(kvp => new LabeledCountDto { Label = kvp.Key, Count = kvp.Value })
                .OrderByDescending(x => x.Count)
                .ToList();

            if (contributorCount > 0)
                usersByRole.Add(new LabeledCountDto { Label = "Contributor", Count = contributorCount });
        }

        // ── Signups this month ─────────────────────────────────────────────────
        var newSignupsThisMonth = await _db.Users.CountAsync(
            u => !u.IsSystemUser && !u.IsDeletedPlaceholder
              && u.AcceptedPrivacyPolicyAtUtc >= startOfMonth
              && u.AcceptedPrivacyPolicyAtUtc < startOfMonth.AddMonths(1), ct);

        // ── Active contributors (distinct submitters in last 30 days) ──────────
        var thirtyDaysAgo = now.AddDays(-30);
        var activeContributors = await _db.Submissions
            .Where(s => s.SubmittedAt >= thirtyDaysAgo)
            .Select(s => s.AccountId)
            .Distinct()
            .CountAsync(ct);

        // ── Contributor retention ──────────────────────────────────────────────
        var submissionCountsPerUser = await _db.Submissions
            .GroupBy(s => s.AccountId)
            .Select(g => g.Count())
            .ToListAsync(ct);

        var repeat = submissionCountsPerUser.Count(c => c > 1);
        var oneTime = submissionCountsPerUser.Count(c => c == 1);

        // ── Top 10 contributors by submission count ────────────────────────────
        var topRaw = await _db.Submissions
            .GroupBy(s => s.AccountId)
            .Select(g => new { AccountId = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(10)
            .ToListAsync(ct);

        List<ContributorSummaryDto> topContributors = new();
        if (topRaw.Count > 0)
        {
            var ids = topRaw.Select(x => x.AccountId).ToList();
            var userMap = await _db.Users
                .Where(u => ids.Contains(u.Id))
                .Select(u => new { u.Id, u.UserName })
                .ToDictionaryAsync(u => u.Id, u => u.UserName, ct);

            topContributors = topRaw.Select(x => new ContributorSummaryDto
            {
                AccountId = x.AccountId.ToString(),
                Username = userMap.GetValueOrDefault(x.AccountId),
                SubmissionCount = x.Count
            }).ToList();
        }

        return new CommunityPanel
        {
            UsersByRole = usersByRole,
            NewSignupsThisMonth = newSignupsThisMonth,
            ActiveContributors = activeContributors,
            ContributorRetention = new ContributorRetentionDto { Repeat = repeat, OneTime = oneTime },
            TopContributors = topContributors
        };
    }
}
