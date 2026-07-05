using AudioAtlasApplication.DTOs.Dashboard;
using AudioAtlasDomain.Enums;
using AudioAtlasDomain.Geography;
using AudioAtlasDomain.Genres;
using AudioAtlasDomain.Search;
using AudioAtlasDomain.Submissions;
using AudioAtlasDomain.Users;
using AudioAtlasInfrastructure.Database;
using AudioAtlasInfrastructure.Services.Dashboard;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AudioAtlasInfrastructureTests;

public class DashboardQueryServiceTests
{
    private static AppDbContext BuildInMemory() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    // ── Catalogue ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task Catalogue_TotalGenres_ReturnsCountOfAllGenres()
    {
        var db = BuildInMemory();
        db.Genres.AddRange(new Genre { Name = "G1" }, new Genre { Name = "G2" }, new Genre { Name = "G3" });
        await db.SaveChangesAsync();

        var svc = new CatalogueQueryService(db);
        var result = await svc.GetAsync(new DashboardFilter());

        Assert.Equal(3, result.TotalGenres);
    }

    [Fact]
    public async Task Catalogue_ContinentFilter_LimitsGenresToMatchingContinent()
    {
        var db = BuildInMemory();
        var africa = new Country { Name = "Ghana", Continent = "Africa", Region = "West Africa", isoCode = "GH" };
        var europe = new Country { Name = "Germany", Continent = "Europe", Region = "Western Europe", isoCode = "DE" };

        var afroGenre = new Genre { Name = "Highlife", Countries = new List<Country> { africa } };
        var euroGenre = new Genre { Name = "Krautrock", Countries = new List<Country> { europe } };
        db.Genres.AddRange(afroGenre, euroGenre);
        await db.SaveChangesAsync();

        var svc = new CatalogueQueryService(db);
        var result = await svc.GetAsync(new DashboardFilter { Continent = "Africa" });

        Assert.Equal(1, result.TotalGenres);
    }

    [Fact]
    public async Task Catalogue_ContentGate_CountsReadyAndNotReady()
    {
        var db = BuildInMemory();
        db.Genres.AddRange(
            new Genre { Name = "G1", Description = "Has description" },
            new Genre { Name = "G2", Description = "" },
            new Genre { Name = "G3", Description = null }
        );
        await db.SaveChangesAsync();

        var svc = new CatalogueQueryService(db);
        var result = await svc.GetAsync(new DashboardFilter());

        Assert.Equal(1, result.ContentGate.Ready);
        Assert.Equal(2, result.ContentGate.NotReady);
    }

    [Fact]
    public async Task Catalogue_DataCompleteness_CountsOrphansAndMissingFields()
    {
        var db = BuildInMemory();
        var country = new Country { Name = "Nigeria", Continent = "Africa", Region = "West Africa", isoCode = "NG" };

        var withEverything = new Genre
        {
            Name = "Afrobeats",
            Description = "A genre",
            PlaylistLink = "https://example.com/playlist",
            Countries = new List<Country> { country }
        };
        var orphanMissingAll = new Genre { Name = "Unknown" };

        db.Genres.AddRange(withEverything, orphanMissingAll);
        await db.SaveChangesAsync();

        var svc = new CatalogueQueryService(db);
        var result = await svc.GetAsync(new DashboardFilter());

        Assert.Equal(1, result.DataCompleteness.OrphanGenres);
        Assert.Equal(1, result.DataCompleteness.MissingOriginsNote);
        Assert.Equal(1, result.DataCompleteness.MissingMedia);
    }

    [Fact]
    public async Task Catalogue_CountryCoverage_GapListContainsCountriesWithNoGenres()
    {
        var db = BuildInMemory();
        var withGenre = new Country { Name = "Brazil", Continent = "South America", Region = "South America", isoCode = "BR" };
        var noGenre = new Country { Name = "Monaco", Continent = "Europe", Region = "Western Europe", isoCode = "MC" };

        db.Genres.Add(new Genre { Name = "Bossa Nova", Countries = new List<Country> { withGenre } });
        db.Countries.Add(noGenre);
        await db.SaveChangesAsync();

        var svc = new CatalogueQueryService(db);
        var result = await svc.GetAsync(new DashboardFilter());

        Assert.Equal(1, result.CountryCoverage.WithGenres);
        Assert.Equal(2, result.CountryCoverage.Total);
        Assert.Contains("Monaco", result.CountryCoverage.GapList);
    }

    [Fact]
    public async Task Catalogue_GenresByContinent_GroupsCorrectly()
    {
        var db = BuildInMemory();
        var africa1 = new Country { Name = "Ghana", Continent = "Africa", Region = "West Africa", isoCode = "GH" };
        var africa2 = new Country { Name = "Nigeria", Continent = "Africa", Region = "West Africa", isoCode = "NG" };
        var europe = new Country { Name = "UK", Continent = "Europe", Region = "Northern Europe", isoCode = "GB" };

        db.Genres.AddRange(
            new Genre { Name = "Highlife", Countries = new List<Country> { africa1 } },
            new Genre { Name = "Afrobeats", Countries = new List<Country> { africa2 } },
            new Genre { Name = "Britpop", Countries = new List<Country> { europe } }
        );
        await db.SaveChangesAsync();

        var svc = new CatalogueQueryService(db);
        var result = await svc.GetAsync(new DashboardFilter());

        var africaRow = result.GenresByContinent.SingleOrDefault(x => x.Label == "Africa");
        var europeRow = result.GenresByContinent.SingleOrDefault(x => x.Label == "Europe");
        Assert.NotNull(africaRow);
        Assert.Equal(2, africaRow.Count);
        Assert.NotNull(europeRow);
        Assert.Equal(1, europeRow.Count);
    }

    // ── Pipeline ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task Pipeline_QueueDepth_CountsPendingSubmissions()
    {
        var db = BuildInMemory();
        db.Submissions.AddRange(
            new Submission { AccountId = Guid.NewGuid(), Status = SubmissionStatus.Pending },
            new Submission { AccountId = Guid.NewGuid(), Status = SubmissionStatus.Pending },
            new Submission { AccountId = Guid.NewGuid(), Status = SubmissionStatus.Approved, ReviewedAt = DateTime.UtcNow }
        );
        await db.SaveChangesAsync();

        var svc = new PipelineQueryService(db);
        var result = await svc.GetAsync(new DashboardFilter());

        Assert.Equal(2, result.QueueDepth);
    }

    [Fact]
    public async Task Pipeline_ApprovalRate_CalculatesCorrectly()
    {
        var db = BuildInMemory();
        var reviewedAt = DateTime.UtcNow;
        db.Submissions.AddRange(
            new Submission { AccountId = Guid.NewGuid(), Status = SubmissionStatus.Approved, ReviewedAt = reviewedAt },
            new Submission { AccountId = Guid.NewGuid(), Status = SubmissionStatus.Approved, ReviewedAt = reviewedAt },
            new Submission { AccountId = Guid.NewGuid(), Status = SubmissionStatus.Rejected, ReviewedAt = reviewedAt, RejectionReasonCode = "duplicate" }
        );
        await db.SaveChangesAsync();

        var svc = new PipelineQueryService(db);
        var result = await svc.GetAsync(new DashboardFilter());

        // 2 approved / 3 total = 66.7%
        Assert.NotNull(result.ApprovalRate);
        Assert.Equal(66.7, result.ApprovalRate!.Value, 1);
    }

    [Fact]
    public async Task Pipeline_ApprovalRate_IsNullWhenNoReviewedSubmissions()
    {
        var db = BuildInMemory();
        db.Submissions.Add(new Submission { AccountId = Guid.NewGuid(), Status = SubmissionStatus.Pending });
        await db.SaveChangesAsync();

        var svc = new PipelineQueryService(db);
        var result = await svc.GetAsync(new DashboardFilter());

        Assert.Null(result.ApprovalRate);
    }

    [Fact]
    public async Task Pipeline_MedianTimeToReview_ComputesCorrectly()
    {
        var db = BuildInMemory();
        var baseTime = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        db.Submissions.AddRange(
            new Submission { AccountId = Guid.NewGuid(), Status = SubmissionStatus.Approved, SubmittedAt = baseTime, ReviewedAt = baseTime.AddHours(2) },
            new Submission { AccountId = Guid.NewGuid(), Status = SubmissionStatus.Approved, SubmittedAt = baseTime, ReviewedAt = baseTime.AddHours(4) },
            new Submission { AccountId = Guid.NewGuid(), Status = SubmissionStatus.Approved, SubmittedAt = baseTime, ReviewedAt = baseTime.AddHours(6) }
        );
        await db.SaveChangesAsync();

        var svc = new PipelineQueryService(db);
        var result = await svc.GetAsync(new DashboardFilter());

        // Median of [2h, 4h, 6h] = 4h
        Assert.Equal(4.0, result.MedianTimeToReviewHours);
    }

    [Fact]
    public async Task Pipeline_FromToFilter_ScopesApprovalRate()
    {
        var db = BuildInMemory();
        var inWindow = new DateTime(2026, 6, 15, 0, 0, 0, DateTimeKind.Utc);
        var outOfWindow = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        db.Submissions.AddRange(
            new Submission { AccountId = Guid.NewGuid(), Status = SubmissionStatus.Approved, ReviewedAt = inWindow },
            new Submission { AccountId = Guid.NewGuid(), Status = SubmissionStatus.Rejected, ReviewedAt = outOfWindow, RejectionReasonCode = "quality" }
        );
        await db.SaveChangesAsync();

        var filter = new DashboardFilter
        {
            From = new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc),
            To = new DateTime(2026, 6, 30, 0, 0, 0, DateTimeKind.Utc)
        };

        var svc = new PipelineQueryService(db);
        var result = await svc.GetAsync(filter);

        // Only the in-window approved submission counts → 100%
        Assert.Equal(100.0, result.ApprovalRate);
    }

    [Fact]
    public async Task Pipeline_RejectionBreakdown_GroupsByCode()
    {
        var db = BuildInMemory();
        var t = DateTime.UtcNow;
        db.Submissions.AddRange(
            new Submission { AccountId = Guid.NewGuid(), Status = SubmissionStatus.Rejected, ReviewedAt = t, RejectionReasonCode = "duplicate" },
            new Submission { AccountId = Guid.NewGuid(), Status = SubmissionStatus.Rejected, ReviewedAt = t, RejectionReasonCode = "duplicate" },
            new Submission { AccountId = Guid.NewGuid(), Status = SubmissionStatus.Rejected, ReviewedAt = t, RejectionReasonCode = "quality" }
        );
        await db.SaveChangesAsync();

        var svc = new PipelineQueryService(db);
        var result = await svc.GetAsync(new DashboardFilter());

        var dup = result.RejectionBreakdown.Single(x => x.Label == "duplicate");
        var qual = result.RejectionBreakdown.Single(x => x.Label == "quality");
        Assert.Equal(2, dup.Count);
        Assert.Equal(1, qual.Count);
    }

    [Fact]
    public async Task Pipeline_GetEarliestReviewAt_ReturnsMinReviewedAt()
    {
        var db = BuildInMemory();
        var early = new DateTime(2025, 3, 1, 0, 0, 0, DateTimeKind.Utc);
        var late = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        db.Submissions.AddRange(
            new Submission { AccountId = Guid.NewGuid(), Status = SubmissionStatus.Approved, ReviewedAt = late },
            new Submission { AccountId = Guid.NewGuid(), Status = SubmissionStatus.Approved, ReviewedAt = early }
        );
        await db.SaveChangesAsync();

        var svc = new PipelineQueryService(db);
        var result = await svc.GetEarliestReviewAtAsync();

        Assert.Equal(early, result);
    }

    // ── Community ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task Community_ContributorRetention_DistinguishesRepeatFromOneTime()
    {
        var db = BuildInMemory();
        var oneTimer = Guid.NewGuid();
        var repeater = Guid.NewGuid();

        db.Submissions.AddRange(
            new Submission { AccountId = oneTimer },
            new Submission { AccountId = repeater },
            new Submission { AccountId = repeater }
        );
        await db.SaveChangesAsync();

        var svc = new CommunityQueryService(db);
        var result = await svc.GetAsync(new DashboardFilter());

        Assert.Equal(1, result.ContributorRetention.Repeat);
        Assert.Equal(1, result.ContributorRetention.OneTime);
    }

    [Fact]
    public async Task Community_TopContributors_RankedBySubmissionCount()
    {
        var db = BuildInMemory();
        var userA = new ApplicationUser { Id = Guid.NewGuid(), UserName = "alice", Email = "a@test.com", SecurityStamp = Guid.NewGuid().ToString() };
        var userB = new ApplicationUser { Id = Guid.NewGuid(), UserName = "bob", Email = "b@test.com", SecurityStamp = Guid.NewGuid().ToString() };
        db.Users.AddRange(userA, userB);

        db.Submissions.AddRange(
            new Submission { AccountId = userA.Id },
            new Submission { AccountId = userA.Id },
            new Submission { AccountId = userA.Id },
            new Submission { AccountId = userB.Id }
        );
        await db.SaveChangesAsync();

        var svc = new CommunityQueryService(db);
        var result = await svc.GetAsync(new DashboardFilter());

        Assert.Equal(2, result.TopContributors.Count);
        Assert.Equal("alice", result.TopContributors[0].Username);
        Assert.Equal(3, result.TopContributors[0].SubmissionCount);
    }

    [Fact]
    public async Task Community_ActiveContributors_CountsDistinctSubmittersInLast30Days()
    {
        var db = BuildInMemory();
        var recentUser = Guid.NewGuid();
        var oldUser = Guid.NewGuid();

        db.Submissions.AddRange(
            new Submission { AccountId = recentUser, SubmittedAt = DateTime.UtcNow.AddDays(-5) },
            new Submission { AccountId = recentUser, SubmittedAt = DateTime.UtcNow.AddDays(-3) },
            new Submission { AccountId = oldUser, SubmittedAt = DateTime.UtcNow.AddDays(-60) }
        );
        await db.SaveChangesAsync();

        var svc = new CommunityQueryService(db);
        var result = await svc.GetAsync(new DashboardFilter());

        Assert.Equal(1, result.ActiveContributors);
    }

    [Fact]
    public async Task Community_UsersByRole_ContributorIsUsersWithNoRole()
    {
        var db = BuildInMemory();
        var roleId = Guid.NewGuid();
        var adminUser = new ApplicationUser { Id = Guid.NewGuid(), UserName = "admin", Email = "admin@test.com", SecurityStamp = Guid.NewGuid().ToString() };
        var plainUser = new ApplicationUser { Id = Guid.NewGuid(), UserName = "user", Email = "user@test.com", SecurityStamp = Guid.NewGuid().ToString() };

        db.Users.AddRange(adminUser, plainUser);
        db.Roles.Add(new IdentityRole<Guid> { Id = roleId, Name = "Admin", NormalizedName = "ADMIN" });
        db.UserRoles.Add(new IdentityUserRole<Guid> { UserId = adminUser.Id, RoleId = roleId });
        await db.SaveChangesAsync();

        var svc = new CommunityQueryService(db);
        var result = await svc.GetAsync(new DashboardFilter());

        var contributor = result.UsersByRole.Single(x => x.Label == "Contributor");
        var admin = result.UsersByRole.Single(x => x.Label == "Admin");
        Assert.Equal(1, contributor.Count);
        Assert.Equal(1, admin.Count);
    }

    // ── Discovery ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task Discovery_ZeroResultSearches_OnlyIncludesZeroResultQueries()
    {
        var db = BuildInMemory();
        db.SearchQueries.AddRange(
            new SearchQuery { Term = "obscure", NormalizedTerm = "obscure", ResultCount = 0, OccurredAt = DateTime.UtcNow },
            new SearchQuery { Term = "obscure", NormalizedTerm = "obscure", ResultCount = 0, OccurredAt = DateTime.UtcNow },
            new SearchQuery { Term = "jazz", NormalizedTerm = "jazz", ResultCount = 5, OccurredAt = DateTime.UtcNow }
        );
        await db.SaveChangesAsync();

        var svc = new DiscoveryQueryService(db);
        var result = await svc.GetAsync(new DashboardFilter());

        Assert.Single(result.ZeroResultSearches);
        Assert.Equal("obscure", result.ZeroResultSearches[0].Term);
        Assert.Equal(2, result.ZeroResultSearches[0].Frequency);
    }

    [Fact]
    public async Task Discovery_TopSearches_RankedByFrequency()
    {
        var db = BuildInMemory();
        db.SearchQueries.AddRange(
            new SearchQuery { Term = "jazz", NormalizedTerm = "jazz", ResultCount = 3, OccurredAt = DateTime.UtcNow },
            new SearchQuery { Term = "jazz", NormalizedTerm = "jazz", ResultCount = 3, OccurredAt = DateTime.UtcNow },
            new SearchQuery { Term = "jazz", NormalizedTerm = "jazz", ResultCount = 3, OccurredAt = DateTime.UtcNow },
            new SearchQuery { Term = "blues", NormalizedTerm = "blues", ResultCount = 2, OccurredAt = DateTime.UtcNow }
        );
        await db.SaveChangesAsync();

        var svc = new DiscoveryQueryService(db);
        var result = await svc.GetAsync(new DashboardFilter());

        Assert.Equal("jazz", result.TopSearches[0].Term);
        Assert.Equal(3, result.TopSearches[0].Frequency);
        Assert.Equal("blues", result.TopSearches[1].Term);
    }

    [Fact]
    public async Task Discovery_ContinentFilter_ScopesSearches()
    {
        var db = BuildInMemory();
        db.SearchQueries.AddRange(
            new SearchQuery { Term = "highlife", NormalizedTerm = "highlife", ResultCount = 0, OccurredAt = DateTime.UtcNow, ContextContinent = "Africa" },
            new SearchQuery { Term = "polka", NormalizedTerm = "polka", ResultCount = 0, OccurredAt = DateTime.UtcNow, ContextContinent = "Europe" }
        );
        await db.SaveChangesAsync();

        var svc = new DiscoveryQueryService(db);
        var result = await svc.GetAsync(new DashboardFilter { Continent = "Africa" });

        Assert.Single(result.ZeroResultSearches);
        Assert.Equal("highlife", result.ZeroResultSearches[0].Term);
    }

    [Fact]
    public async Task Discovery_FromToFilter_ScopesSearches()
    {
        var db = BuildInMemory();
        var inWindow = new DateTime(2026, 6, 15, 0, 0, 0, DateTimeKind.Utc);
        var outOfWindow = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        db.SearchQueries.AddRange(
            new SearchQuery { Term = "recent", NormalizedTerm = "recent", ResultCount = 0, OccurredAt = inWindow },
            new SearchQuery { Term = "old", NormalizedTerm = "old", ResultCount = 0, OccurredAt = outOfWindow }
        );
        await db.SaveChangesAsync();

        var filter = new DashboardFilter
        {
            From = new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc),
            To = new DateTime(2026, 6, 30, 0, 0, 0, DateTimeKind.Utc)
        };

        var svc = new DiscoveryQueryService(db);
        var result = await svc.GetAsync(filter);

        Assert.Single(result.ZeroResultSearches);
        Assert.Equal("recent", result.ZeroResultSearches[0].Term);
    }
}
