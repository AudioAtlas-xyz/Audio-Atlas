using AudioAtlasApplication.DTOs;
using AudioAtlasDomain.Enums;
using AudioAtlasDomain.Geography;
using AudioAtlasDomain.Genres;
using AudioAtlasDomain.Submissions;
using AudioAtlasDomain.Users;
using AudioAtlasInfrastructure.Database;
using AudioAtlasInfrastructure.Services;
using Microsoft.EntityFrameworkCore;
using InfrastructureCountryRepository = AudioAtlasInfrastructure.Repositories.CountryRepository;
using InfrastructureGenreRepository = AudioAtlasInfrastructure.Repositories.GenreRepository;
using InfrastructureSubmissionRepository = AudioAtlasInfrastructure.Repositories.SubmissionRepository;
using InfrastructureInstrumentRepository = AudioAtlasInfrastructure.Repositories.InstrumentRepository;
using InfrastructureSubmissionService = AudioAtlasInfrastructure.Services.SubmissionService;

namespace AudioAtlasInfrastructureTests;

public class SubmissionServiceTests
{
    private static (AppDbContext db, SubmissionService service) BuildInMemory()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var db = new AppDbContext(options);

        var service = new InfrastructureSubmissionService(
            new InfrastructureCountryRepository(db),
            new InfrastructureGenreRepository(db),
            new InfrastructureInstrumentRepository(db),
            new InfrastructureSubmissionRepository(db));

        return (db, service);
    }

    private static void SeedActiveRejectionReason(AppDbContext db, string code = "duplicate")
    {
        db.RejectionReasons.Add(new RejectionReason
        {
            Code = code,
            Label = "Already exists in the atlas",
            GuidelineRef = "data-integrity",
            SortOrder = 1,
            IsActive = true
        });
        db.SaveChanges();
    }

    private static Submission SeedPendingSubmission(AppDbContext db)
    {
        var submission = new Submission
        {
            Id = Guid.NewGuid(),
            AccountId = Guid.NewGuid(),
            NewGenreName = "Pending Genre",
            Description = "Pending description",
            Sources = [new SubmissionSource { SourceLink = "https://example.com/source" }]
        };
        db.Submissions.Add(submission);
        db.SaveChanges();
        return submission;
    }

    [Fact]
    public async Task CreateSubmissionAsync_WithValidRequest_PersistsSubmission()
    {
        var (db, service) = BuildInMemory();

        var account = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = "submitter",
            Email = "submitter@test.com"
        };

        var country = new Country
        {
            Id = Guid.NewGuid(),
            Name = "Denmark",
            Region = "Northern Europe",
            Continent = "Europe",
            isoCode = "DNK"
        };

        var relatedGenre = new Genre
        {
            Id = Guid.NewGuid(),
            Name = "Folk"
        };

        db.Users.Add(account);
        db.Countries.Add(country);
        db.Genres.Add(relatedGenre);
        await db.SaveChangesAsync();

        var command = new CreateSubmissionRequest
        {
            NewGenreName = "Nordic Wave",
            Description = "A proposal for a contemporary Nordic crossover genre.",
            IsSensitive = false,
            ExampleSongYoutubeId = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
            Aliases = ["North Wave", "Nordic Wave"],
            SourceLinks = ["https://example.com/source-1", "https://example.com/source-1"],
            CountryIds = [country.Id],
            SimilarGenreIds = [relatedGenre.Id],
            SubGenreIds = [relatedGenre.Id],
            PredecessorGenreIds = [relatedGenre.Id]
        };

        var submissionId = await service.createSubmissionAsync(account.Id, command);

        var submission = await db.Submissions
            .Include(x => x.Aliases)
            .Include(x => x.Sources)
            .Include(x => x.Countries)
            .Include(x => x.SimilarGenres)
            .Include(x => x.SubGenres)
            .Include(x => x.PredecessorGenres)
            .SingleAsync(x => x.Id == submissionId);

        Assert.Equal(account.Id, submission.AccountId);
        Assert.Equal("Nordic Wave", submission.NewGenreName);
        Assert.Equal("A proposal for a contemporary Nordic crossover genre.", submission.Description);
        // Stored as the bare video id, not the URL that was submitted.
        Assert.Equal("dQw4w9WgXcQ", submission.ExampleSongYoutubeId);
        Assert.Equal(2, submission.Aliases.Count);
        Assert.Single(submission.Sources);
        Assert.Single(submission.Countries);
        Assert.Single(submission.SimilarGenres);
        Assert.Single(submission.SubGenres);
        Assert.Single(submission.PredecessorGenres);
    }

    [Fact]
    public async Task CreateSubmissionAsync_WhenRequiredFieldsMissing_ThrowsInvalidOperationException()
    {
        var (_, service) = BuildInMemory();

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.createSubmissionAsync(Guid.NewGuid(), new CreateSubmissionRequest
            {
                Description = "",
                SourceLinks = []
            }));

        Assert.Contains("newGenreName", exception.Message);
        Assert.Contains("description", exception.Message);
    }

    [Fact]
    public async Task CreateSubmissionAsync_WhenNoCountryGiven_ThrowsInvalidOperationException()
    {
        // An approved submission becomes a genre. Without a country that genre is
        // invisible on the globe and in every geography-scoped query, so the
        // submission must not be accepted in the first place.
        var (_, service) = BuildInMemory();

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.createSubmissionAsync(Guid.NewGuid(), new CreateSubmissionRequest
            {
                NewGenreName = "Nordic Wave",
                Description = "A proposal for a contemporary Nordic crossover genre.",
                SourceLinks = ["https://example.com/source-1"],
                CountryIds = []
            }));

        Assert.Contains("countryIds", exception.Message);
    }

    [Fact]
    public async Task CreateSubmissionAsync_WhenExampleSongIsNotYouTube_ThrowsInvalidOperationException()
    {
        // The stored value is used to build an iframe URL, so the submission path
        // has to reject anything that is not a YouTube video outright.
        var (db, service) = BuildInMemory();

        var country = new Country
        {
            Id = Guid.NewGuid(), Name = "Denmark", Region = "Northern Europe",
            Continent = "Europe", isoCode = "DNK"
        };
        db.Countries.Add(country);
        await db.SaveChangesAsync();

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.createSubmissionAsync(Guid.NewGuid(), new CreateSubmissionRequest
            {
                NewGenreName = "Nordic Wave",
                Description = "A proposal for a contemporary Nordic crossover genre.",
                SourceLinks = ["https://example.com/source-1"],
                CountryIds = [country.Id],
                ExampleSongYoutubeId = "https://open.spotify.com/playlist/37i9dQZF1DX"
            }));

        Assert.Contains("exampleSongYoutubeId", exception.Message);
    }

    [Fact]
    public async Task CreateSubmissionAsync_WhenReferencedIdsDoNotExist_ThrowsInvalidOperationException()
    {
        var (_, service) = BuildInMemory();

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.createSubmissionAsync(Guid.NewGuid(), new CreateSubmissionRequest
            {
                NewGenreName = "Nordic Wave",
                Description = "A proposal",
                SourceLinks = ["https://example.com/source-1"],
                CountryIds = [Guid.NewGuid()],
                SimilarGenreIds = [Guid.NewGuid()]
            }));

        Assert.Contains("countryIds", exception.Message);
        Assert.Contains("similarGenreIds", exception.Message);
    }

    [Fact]
    public async Task GetPendingAsync_ReturnsOnlyNonRejectedSubmissions()
    {
        var (db, service) = BuildInMemory();

        var account = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = "reviewer",
            Email = "reviewer@test.com"
        };

        var country = new Country
        {
            Id = Guid.NewGuid(),
            Name = "Denmark",
            Region = "Northern Europe",
            Continent = "Europe",
            isoCode = "DNK"
        };

        var genre = new Genre
        {
            Id = Guid.NewGuid(),
            Name = "Folk"
        };

        db.Users.Add(account);
        db.Countries.Add(country);
        db.Genres.Add(genre);

        db.Submissions.Add(new Submission
        {
            AccountId = account.Id,
            Account = account,
            NewGenreName = "Pending Genre",
            Description = "Pending description",
            Sources = [new SubmissionSource { SourceLink = "https://example.com/pending-source" }],
            Aliases = [new SubmissionAlias { Alias = "Pending Alias" }],
            Countries = [country],
            SimilarGenres = [genre]
        });

        db.Submissions.Add(new Submission
        {
            AccountId = account.Id,
            Account = account,
            NewGenreName = "Rejected Genre",
            Description = "Rejected description",
            Status = SubmissionStatus.Rejected,
            Sources = [new SubmissionSource { SourceLink = "https://example.com/rejected-source" }]
        });

        await db.SaveChangesAsync();

        var submissions = await service.getPendingAsync();

        var submission = Assert.Single(submissions);
        Assert.Equal("Pending Genre", submission.NewGenreName);
        Assert.Equal(account.Id, submission.AccountId);
        Assert.Equal("reviewer", submission.AccountUsername);
        Assert.Single(submission.Aliases);
        Assert.Single(submission.SourceLinks);
        Assert.Single(submission.CountryIds);
        Assert.Single(submission.SimilarGenreIds);
    }

    [Fact]
    public async Task ApproveAsync_MarksSubmissionAsApproved()
    {
        var (db, service) = BuildInMemory();
        var submission = SeedPendingSubmission(db);

        await service.approveAsync(submission.Id, Guid.NewGuid());

        var approved = await db.Submissions.SingleAsync(x => x.Id == submission.Id);
        Assert.Equal(SubmissionStatus.Approved, approved.Status);
    }

    [Fact]
    public async Task ApproveAsync_StampsReviewedAtAndReviewedById()
    {
        var (db, service) = BuildInMemory();
        var submission = SeedPendingSubmission(db);
        var reviewerId = Guid.NewGuid();
        var before = DateTime.UtcNow;

        await service.approveAsync(submission.Id, reviewerId);

        var approved = await db.Submissions.SingleAsync(x => x.Id == submission.Id);
        Assert.Equal(reviewerId, approved.ReviewedById);
        Assert.NotNull(approved.ReviewedAt);
        Assert.True(approved.ReviewedAt >= before);
    }

    [Fact]
    public async Task ApproveAsync_CanApproveOnHoldSubmission()
    {
        var (db, service) = BuildInMemory();
        var reviewerId = Guid.NewGuid();

        var submission = new Submission
        {
            Id = Guid.NewGuid(),
            AccountId = Guid.NewGuid(),
            NewGenreName = "On-Hold Genre",
            Description = "Sensitive description",
            Status = SubmissionStatus.OnHoldSensitivity,
            Sources = [new SubmissionSource { SourceLink = "https://example.com/source" }]
        };
        db.Submissions.Add(submission);
        await db.SaveChangesAsync();

        await service.approveAsync(submission.Id, reviewerId);

        var approved = await db.Submissions.SingleAsync(x => x.Id == submission.Id);
        Assert.Equal(SubmissionStatus.Approved, approved.Status);
        Assert.Equal(reviewerId, approved.ReviewedById);
    }

    [Fact]
    public async Task RejectAsync_WithValidReasonCode_MarksRejectedAndStampsFields()
    {
        var (db, service) = BuildInMemory();
        var submission = SeedPendingSubmission(db);
        SeedActiveRejectionReason(db, "duplicate");
        var reviewerId = Guid.NewGuid();
        var before = DateTime.UtcNow;

        await service.rejectAsync(submission.Id, reviewerId, new RejectSubmissionRequest
        {
            RejectionReasonCode = "duplicate",
            Note = "Exact copy already in atlas."
        });

        var rejected = await db.Submissions
            .Include(x => x.RejectedSubmission)
            .SingleAsync(x => x.Id == submission.Id);

        Assert.Equal(SubmissionStatus.Rejected, rejected.Status);
        Assert.Equal(reviewerId, rejected.ReviewedById);
        Assert.NotNull(rejected.ReviewedAt);
        Assert.True(rejected.ReviewedAt >= before);
        Assert.Equal("duplicate", rejected.RejectionReasonCode);
        Assert.NotNull(rejected.RejectedSubmission);
        Assert.Equal("Exact copy already in atlas.", rejected.RejectedSubmission!.Description);
    }

    [Fact]
    public async Task RejectAsync_WithValidReasonCodeAndNoNote_StoresEmptyDescription()
    {
        var (db, service) = BuildInMemory();
        var submission = SeedPendingSubmission(db);
        SeedActiveRejectionReason(db, "duplicate");

        await service.rejectAsync(submission.Id, Guid.NewGuid(), new RejectSubmissionRequest
        {
            RejectionReasonCode = "duplicate"
        });

        var rejected = await db.Submissions
            .Include(x => x.RejectedSubmission)
            .SingleAsync(x => x.Id == submission.Id);

        Assert.Equal(SubmissionStatus.Rejected, rejected.Status);
        Assert.Equal("duplicate", rejected.RejectionReasonCode);
        Assert.NotNull(rejected.RejectedSubmission);
        Assert.Equal(string.Empty, rejected.RejectedSubmission!.Description);
    }

    [Fact]
    public async Task RejectAsync_WithoutReasonCode_ThrowsInvalidOperationException()
    {
        var (db, service) = BuildInMemory();
        var submission = SeedPendingSubmission(db);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.rejectAsync(submission.Id, Guid.NewGuid(), new RejectSubmissionRequest
            {
                RejectionReasonCode = ""
            }));

        Assert.Contains("rejectionReasonCode", exception.Message);
    }

    [Fact]
    public async Task RejectAsync_WithUnknownReasonCode_ThrowsInvalidOperationException()
    {
        var (db, service) = BuildInMemory();
        var submission = SeedPendingSubmission(db);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.rejectAsync(submission.Id, Guid.NewGuid(), new RejectSubmissionRequest
            {
                RejectionReasonCode = "nonexistent_code"
            }));

        Assert.Contains("rejectionReasonCode", exception.Message);
    }

    [Fact]
    public async Task RejectAsync_WithInactiveReasonCode_ThrowsInvalidOperationException()
    {
        var (db, service) = BuildInMemory();
        var submission = SeedPendingSubmission(db);

        db.RejectionReasons.Add(new RejectionReason
        {
            Code = "deprecated_reason",
            Label = "Old reason",
            SortOrder = 99,
            IsActive = false
        });
        await db.SaveChangesAsync();

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.rejectAsync(submission.Id, Guid.NewGuid(), new RejectSubmissionRequest
            {
                RejectionReasonCode = "deprecated_reason"
            }));

        Assert.Contains("rejectionReasonCode", exception.Message);
    }

    [Fact]
    public async Task HoldForSensitivityAsync_SetsOnHoldStatusAndStampsFields()
    {
        var (db, service) = BuildInMemory();
        var submission = SeedPendingSubmission(db);
        var reviewerId = Guid.NewGuid();
        var before = DateTime.UtcNow;

        await service.holdForSensitivityAsync(submission.Id, reviewerId);

        var held = await db.Submissions.SingleAsync(x => x.Id == submission.Id);
        Assert.Equal(SubmissionStatus.OnHoldSensitivity, held.Status);
        Assert.Equal(reviewerId, held.ReviewedById);
        Assert.NotNull(held.ReviewedAt);
        Assert.True(held.ReviewedAt >= before);
    }

    [Fact]
    public async Task HoldForSensitivityAsync_WhenAlreadyOnHold_ThrowsInvalidOperationException()
    {
        var (db, service) = BuildInMemory();

        var submission = new Submission
        {
            Id = Guid.NewGuid(),
            AccountId = Guid.NewGuid(),
            NewGenreName = "Held Genre",
            Description = "Already held",
            Status = SubmissionStatus.OnHoldSensitivity,
            Sources = [new SubmissionSource { SourceLink = "https://example.com/source" }]
        };
        db.Submissions.Add(submission);
        await db.SaveChangesAsync();

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.holdForSensitivityAsync(submission.Id, Guid.NewGuid()));

        Assert.Contains("already on hold", exception.Message);
    }

    [Fact]
    public async Task GetActiveRejectionReasonsAsync_ReturnsOnlyActiveReasonsOrderedBySortOrder()
    {
        var (db, service) = BuildInMemory();

        db.RejectionReasons.AddRange(
            new RejectionReason { Code = "c", Label = "Third", SortOrder = 3, IsActive = true },
            new RejectionReason { Code = "a", Label = "First", SortOrder = 1, IsActive = true },
            new RejectionReason { Code = "b", Label = "Second", SortOrder = 2, IsActive = true },
            new RejectionReason { Code = "d", Label = "Inactive", SortOrder = 4, IsActive = false }
        );
        await db.SaveChangesAsync();

        var reasons = await service.getActiveRejectionReasonsAsync();

        Assert.Equal(3, reasons.Count);
        var list = reasons.ToList();
        Assert.Equal("a", list[0].Code);
        Assert.Equal("b", list[1].Code);
        Assert.Equal("c", list[2].Code);
    }
}
