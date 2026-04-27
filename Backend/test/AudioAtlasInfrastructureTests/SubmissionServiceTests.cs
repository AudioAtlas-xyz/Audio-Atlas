using AudioAtlasApplication.DTOs;
using AudioAtlasApplication.Services;
using AudioAtlasDomain.Geography;
using AudioAtlasDomain.Genres;
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
    public async Task CreateSubmissionAsync_WhenRequiredFieldsMissing_ThrowsValidationException()
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

        var exception = await Assert.ThrowsAsync<SubmissionValidationException>(() =>
            service.createSubmissionAsync(Guid.NewGuid(), command));

        Assert.Contains("newGenreName", exception.Errors.Keys);
        Assert.Contains("description", exception.Errors.Keys);
        Assert.Contains("sourceLinks", exception.Errors.Keys);
    }

    [Fact]
    public async Task CreateSubmissionAsync_WhenReferencedIdsDoNotExist_ThrowsValidationException()
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

        var exception = await Assert.ThrowsAsync<SubmissionValidationException>(() =>
            service.createSubmissionAsync(Guid.NewGuid(), command));

        Assert.Contains("countryIds", exception.Errors.Keys);
        Assert.Contains("similarGenreIds", exception.Errors.Keys);
    }
}
