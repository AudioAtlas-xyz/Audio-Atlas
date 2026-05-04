using AudioAtlasApplication.DTOs;
using AudioAtlasDomain.Geography;
using AudioAtlasDomain.Genres;
using AudioAtlasDomain.Submissions;
using AudioAtlasDomain.Users;
using AudioAtlasInfrastructure.Database;
using AudioAtlasInfrastructure.Repositories;
using AudioAtlasInfrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace AudioAtlasInfrastructureTests;

public class SubmissionServiceTests
{
    [Fact]
    public async Task CreateSubmissionAsync_WithValidRequest_PersistsSubmission()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var dbContext = new AppDbContext(options);

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

        dbContext.Users.Add(account);
        dbContext.Countries.Add(country);
        dbContext.Genres.Add(relatedGenre);
        await dbContext.SaveChangesAsync();

        var submissionRepository = new SubmissionRepository(dbContext);
        var countryRepository = new CountryRepository(dbContext);
        var genreRepository = new GenreRepository(dbContext);
        var service = new SubmissionService(countryRepository, genreRepository, submissionRepository);

        var command = new CreateSubmissionRequest
        {
            NewGenreName = "Nordic Wave",
            Description = "A proposal for a contemporary Nordic crossover genre.",
            IsSensitive = false,
            PlaylistLink = "https://example.com/playlist",
            Aliases = ["North Wave", "Nordic Wave"],
            SourceLinks = ["https://example.com/source-1", "https://example.com/source-1"],
            CountryIds = [country.Id],
            SimilarGenreIds = [relatedGenre.Id],
            SubGenreIds = [relatedGenre.Id],
            PredecessorGenreIds = [relatedGenre.Id]
        };

        var submissionId = await service.createSubmissionAsync(account.Id, command);

        var submission = await dbContext.Submissions
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
        Assert.Equal("https://example.com/playlist", submission.PlaylistLink);
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
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var dbContext = new AppDbContext(options);
        var submissionRepository = new SubmissionRepository(dbContext);
        var countryRepository = new CountryRepository(dbContext);
        var genreRepository = new GenreRepository(dbContext);
        var service = new SubmissionService(countryRepository, genreRepository, submissionRepository);

        var command = new CreateSubmissionRequest
        {
            Description = "",
            SourceLinks = []
        };

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.createSubmissionAsync(Guid.NewGuid(), command));

        Assert.Contains("newGenreName", exception.Message);
        Assert.Contains("description", exception.Message);
        Assert.Contains("sourceLinks", exception.Message);
    }

    [Fact]
    public async Task CreateSubmissionAsync_WhenReferencedIdsDoNotExist_ThrowsInvalidOperationException()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var dbContext = new AppDbContext(options);
        var submissionRepository = new SubmissionRepository(dbContext);
        var countryRepository = new CountryRepository(dbContext);
        var genreRepository = new GenreRepository(dbContext);
        var service = new SubmissionService(countryRepository, genreRepository, submissionRepository);

        var command = new CreateSubmissionRequest
        {
            NewGenreName = "Nordic Wave",
            Description = "A proposal",
            SourceLinks = ["https://example.com/source-1"],
            CountryIds = [Guid.NewGuid()],
            SimilarGenreIds = [Guid.NewGuid()]
        };

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.createSubmissionAsync(Guid.NewGuid(), command));

        Assert.Contains("countryIds", exception.Message);
        Assert.Contains("similarGenreIds", exception.Message);
    }

    [Fact]
    public async Task GetPendingAsync_ReturnsOnlyNonRejectedSubmissions()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var dbContext = new AppDbContext(options);

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

        dbContext.Users.Add(account);
        dbContext.Countries.Add(country);
        dbContext.Genres.Add(genre);

        dbContext.Submissions.Add(new Submission
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

        dbContext.Submissions.Add(new Submission
        {
            AccountId = account.Id,
            Account = account,
            NewGenreName = "Rejected Genre",
            Description = "Rejected description",
            IsRejected = true,
            Sources = [new SubmissionSource { SourceLink = "https://example.com/rejected-source" }]
        });

        await dbContext.SaveChangesAsync();

        var submissionRepository = new SubmissionRepository(dbContext);
        var countryRepository = new CountryRepository(dbContext);
        var genreRepository = new GenreRepository(dbContext);
        var service = new SubmissionService(countryRepository, genreRepository, submissionRepository);

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
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var dbContext = new AppDbContext(options);

        var submission = new Submission
        {
            Id = Guid.NewGuid(),
            AccountId = Guid.NewGuid(),
            NewGenreName = "Pending Genre",
            Description = "Pending description",
            Sources = [new SubmissionSource { SourceLink = "https://example.com/source" }]
        };

        dbContext.Submissions.Add(submission);
        await dbContext.SaveChangesAsync();

        var service = new SubmissionService(
            new CountryRepository(dbContext),
            new GenreRepository(dbContext),
            new SubmissionRepository(dbContext));

        await service.approveAsync(submission.Id);

        var approvedSubmission = await dbContext.Submissions.SingleAsync(x => x.Id == submission.Id);
        Assert.True(approvedSubmission.IsApproved);
        Assert.False(approvedSubmission.IsRejected);
    }

    [Fact]
    public async Task RejectAsync_MarksSubmissionAsRejectedAndStoresReason()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var dbContext = new AppDbContext(options);

        var submission = new Submission
        {
            Id = Guid.NewGuid(),
            AccountId = Guid.NewGuid(),
            NewGenreName = "Pending Genre",
            Description = "Pending description",
            Sources = [new SubmissionSource { SourceLink = "https://example.com/source" }]
        };

        dbContext.Submissions.Add(submission);
        await dbContext.SaveChangesAsync();

        var service = new SubmissionService(
            new CountryRepository(dbContext),
            new GenreRepository(dbContext),
            new SubmissionRepository(dbContext));

        await service.rejectAsync(submission.Id, new RejectSubmissionRequest
        {
            Reason = "Not enough supporting evidence."
        });

        var rejectedSubmission = await dbContext.Submissions
            .Include(x => x.RejectedSubmission)
            .SingleAsync(x => x.Id == submission.Id);

        Assert.True(rejectedSubmission.IsRejected);
        Assert.False(rejectedSubmission.IsApproved);
        Assert.NotNull(rejectedSubmission.RejectedSubmission);
        Assert.Equal("Not enough supporting evidence.", rejectedSubmission.RejectedSubmission!.Description);
    }
}
