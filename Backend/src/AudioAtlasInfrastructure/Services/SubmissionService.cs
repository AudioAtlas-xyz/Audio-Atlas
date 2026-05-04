using AudioAtlasApplication.DTOs;
using AudioAtlasApplication.Repositories;
using AudioAtlasApplication.Services;
using AudioAtlasDomain.Enums;
using AudioAtlasDomain.Submissions;

namespace AudioAtlasInfrastructure.Services;

public class SubmissionService : ISubmissionService
{
    private const int MaxNameLength = 120;
    private const int MaxAliasLength = 120;
    private const int MaxDescriptionLength = 4000;
    private const int MaxUrlLength = 2048;
    private const int MaxRejectReasonLength = 2000;

    private readonly ICountryRepository _countryRepository;
    private readonly IGenreRepository _genreRepository;
    private readonly ISubmissionRepository _submissionRepository;

    public SubmissionService(
        ICountryRepository countryRepository,
        IGenreRepository genreRepository,
        ISubmissionRepository submissionRepository)
    {
        _countryRepository = countryRepository;
        _genreRepository = genreRepository;
        _submissionRepository = submissionRepository;
    }

    // Create a submission and check wether the input format is  correct
    public async Task<Guid> createSubmissionAsync(Guid accountId, CreateSubmissionRequest request, CancellationToken cancellationToken = default)
    {
        var errors = new Dictionary<string, string[]>();

        var normalizedGenreName = normalizeText(request.NewGenreName);
        var normalizedDescription = normalizeText(request.Description);
        var normalizedPlaylistLink = normalizeOptionalUrl(request.PlaylistLink, "playlistLink", errors);
        var aliasValues = normalizeDistinctTexts(request.Aliases ?? []);
        var normalizedSourceLinks = normalizeDistinctUrls(request.SourceLinks ?? [], "sourceLinks", errors);
        var countryIds = distinctIds(request.CountryIds ?? []);
        var similarGenreIds = distinctIds(request.SimilarGenreIds ?? []);
        var subGenreIds = distinctIds(request.SubGenreIds ?? []);
        var predecessorGenreIds = distinctIds(request.PredecessorGenreIds ?? []);

        // --- Exception checks ---

        // Genre name exists and is no more than max length
        if (string.IsNullOrWhiteSpace(normalizedGenreName))
        {
            errors["newGenreName"] = ["New genre name is required."];
        }
        else if (normalizedGenreName.Length > MaxNameLength)
        {
            errors["newGenreName"] = [$"New genre name must be at most {MaxNameLength} characters."];
        }

        // description is included and no more than max length
        if (string.IsNullOrWhiteSpace(normalizedDescription))
        {
            errors["description"] = ["Description is required."];
        }
        else if (normalizedDescription.Length > MaxDescriptionLength)
        {
            errors["description"] = [$"Description must be at most {MaxDescriptionLength} characters."];
        }

        // Start date must be before end date
        if (request.StartDate.HasValue && request.EndDate.HasValue && request.StartDate.Value > request.EndDate.Value)
        {
            errors["dateRange"] = ["Start date must be earlier than or equal to end date."];
        }

        // aliases max length
        if (aliasValues.Any(alias => alias.Length > MaxAliasLength))
        {
            errors["aliases"] = [$"Aliases must be at most {MaxAliasLength} characters."];
        }

        // at least one source link
        if (normalizedSourceLinks.Count == 0)
        {
            errors["sourceLinks"] = ["At least one source link is required."];
        }

        // max playlist url length
        if (normalizedPlaylistLink is { Length: > MaxUrlLength })
        {
            errors["playlistLink"] = [$"Playlist link must be at most {MaxUrlLength} characters."];
        }

        // max source url length
        if (normalizedSourceLinks.Any(link => link.Length > MaxUrlLength))
        {
            errors["sourceLinks"] = [$"Source links must be at most {MaxUrlLength} characters."];
        }

        var normalizedAliases = aliasValues
            .Where(alias => alias.Length <= MaxAliasLength)
            .ToList();

        // throw all errors
        if (errors.Count > 0)
        {
            throw new InvalidOperationException(buildErrorMessage(errors));
        }

        // get specified countried
        var countries = await _countryRepository.getCountriesByIdsAsync(countryIds, cancellationToken);
        var similarGenres = await _genreRepository.getGenresByIdsAsync(similarGenreIds, cancellationToken);
        var subGenres = await _genreRepository.getGenresByIdsAsync(subGenreIds, cancellationToken);
        var predecessorGenres = await _genreRepository.getGenresByIdsAsync(predecessorGenreIds, cancellationToken);

        // Do they exist?
        addMissingIdErrors(errors, "countryIds", countryIds, countries.Select(country => country.Id));
        addMissingIdErrors(errors, "similarGenreIds", similarGenreIds, similarGenres.Select(genre => genre.Id));
        addMissingIdErrors(errors, "subGenreIds", subGenreIds, subGenres.Select(genre => genre.Id));
        addMissingIdErrors(errors, "predecessorGenreIds", predecessorGenreIds, predecessorGenres.Select(genre => genre.Id));

        // throw exceptions
        if (errors.Count > 0)
        {
            throw new InvalidOperationException(buildErrorMessage(errors));
        }

        // all checks passed, create the submission object
        var submission = new Submission
        {
            AccountId = accountId,
            NewGenreName = normalizedGenreName,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Description = normalizedDescription,
            IsSensitive = request.IsSensitive,
            PlaylistLink = normalizedPlaylistLink,
            Status = SubmissionStatus.Pending,
            Aliases = normalizedAliases.Select(alias => new SubmissionAlias
            {
                Alias = alias
            }).ToList(),
            Sources = normalizedSourceLinks.Select(link => new SubmissionSource
            {
                SourceLink = link
            }).ToList(),
            Countries = countries.ToList(),
            SimilarGenres = similarGenres.ToList(),
            SubGenres = subGenres.ToList(),
            PredecessorGenres = predecessorGenres.ToList()
        };

        await _submissionRepository.addAsync(submission, cancellationToken);

        return submission.Id;
    }

    public async Task<ICollection<PendingSubmissionResponse>> getPendingAsync(CancellationToken cancellationToken = default)
    {
        var submissions = await _submissionRepository.getPendingAsync(cancellationToken);

        return submissions
            .Select(submission => new PendingSubmissionResponse
            {
                Id = submission.Id,
                AccountId = submission.AccountId,
                AccountUsername = submission.Account?.UserName,
                NewGenreName = submission.NewGenreName,
                StartDate = submission.StartDate,
                EndDate = submission.EndDate,
                Description = submission.Description,
                IsSensitive = submission.IsSensitive,
                PlaylistLink = submission.PlaylistLink,
                Aliases = submission.Aliases.Select(alias => alias.Alias).ToList(),
                SourceLinks = submission.Sources.Select(source => source.SourceLink).ToList(),
                CountryIds = submission.Countries.Select(country => country.Id).ToList(),
                SimilarGenreIds = submission.SimilarGenres.Select(genre => genre.Id).ToList(),
                SubGenreIds = submission.SubGenres.Select(genre => genre.Id).ToList(),
                PredecessorGenreIds = submission.PredecessorGenres.Select(genre => genre.Id).ToList()
            })
            .ToList();
    }

    public async Task approveAsync(Guid submissionId, CancellationToken cancellationToken = default)
    {
        var submission = await getPendingSubmissionOrThrowAsync(submissionId, cancellationToken);

        submission.Status = SubmissionStatus.Approved;
        submission.RejectedSubmission = null;

        await _submissionRepository.saveChangesAsync(cancellationToken);
    }

    public async Task rejectAsync(Guid submissionId, RejectSubmissionRequest request, CancellationToken cancellationToken = default)
    {
        var submission = await getPendingSubmissionOrThrowAsync(submissionId, cancellationToken);

        var normalizedReason = normalizeText(request.Reason);

        if (string.IsNullOrWhiteSpace(normalizedReason))
        {
            throw new InvalidOperationException("reason: Rejection reason is required.");
        }

        if (normalizedReason.Length > MaxRejectReasonLength)
        {
            throw new InvalidOperationException($"reason: Rejection reason must be at most {MaxRejectReasonLength} characters.");
        }

        submission.Status = SubmissionStatus.Rejected;

        if (submission.RejectedSubmission is null)
        {
            submission.RejectedSubmission = new RejectedSubmission
            {
                SubmissionId = submission.Id,
                Description = normalizedReason
            };
        }
        else
        {
            submission.RejectedSubmission.Description = normalizedReason;
        }

        await _submissionRepository.saveChangesAsync(cancellationToken);
    }

    //  checks if all IDs from the request in the database. Outputs which werent found. 
    private static void addMissingIdErrors(
        Dictionary<string, string[]> errors,
        string key,
        IReadOnlyCollection<Guid> requestedIds,
        IEnumerable<Guid> resolvedIds)
    {
        var resolvedSet = resolvedIds.ToHashSet();
        var missingIds = requestedIds.Where(id => !resolvedSet.Contains(id)).ToArray();

        if (missingIds.Length > 0)
        {
            errors[key] = [$"Unknown ids: {string.Join(", ", missingIds)}"];
        }
    }


    // --- Helpers for input mismatch and normalization ---
    private static string? normalizeText(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static string? normalizeOptionalUrl(string? value, string fieldName, Dictionary<string, string[]> errors)
    {
        var normalized = normalizeText(value);

        if (normalized is null)
        {
            return null;
        }

        if (!isHttpUrl(normalized))
        {
            errors[fieldName] = ["Value must be an absolute http or https URL."];
        }

        return normalized;
    }

    private static List<string> normalizeDistinctTexts(IEnumerable<string> values)
    {
        return values
            .Select(normalizeText)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static List<string> normalizeDistinctUrls(IEnumerable<string> values, string fieldName, Dictionary<string, string[]> errors)
    {
        var invalidUrls = new List<string>();
        var validUrls = new List<string>();

        foreach (var value in values.Select(normalizeText).Where(value => !string.IsNullOrWhiteSpace(value)).Select(value => value!))
        {
            if (!isHttpUrl(value))
            {
                invalidUrls.Add(value);
                continue;
            }

            validUrls.Add(value);
        }

        if (invalidUrls.Count > 0)
        {
            errors[fieldName] = ["All source links must be absolute http or https URLs."];
        }

        return validUrls
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static List<Guid> distinctIds(IEnumerable<Guid> values)
    {
        return values
            .Where(id => id != Guid.Empty)
            .Distinct()
            .ToList();
    }

    private static bool isHttpUrl(string value)
    {
        return Uri.TryCreate(value, UriKind.Absolute, out var uri) &&
               (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }

    private static string buildErrorMessage(IReadOnlyDictionary<string, string[]> errors)
    {
        return string.Join(
            " | ",
            errors.SelectMany(error => error.Value.Select(message => $"{error.Key}: {message}")));
    }

    private async Task<Submission> getPendingSubmissionOrThrowAsync(Guid submissionId, CancellationToken cancellationToken)
    {
        var submission = await _submissionRepository.getByIdAsync(submissionId, cancellationToken);

        if (submission is null)
        {
            throw new InvalidOperationException("submissionId: Submission not found.");
        }

        if (submission.Status == SubmissionStatus.Approved)
        {
            throw new InvalidOperationException("submissionId: Submission has already been approved.");
        }

        if (submission.Status == SubmissionStatus.Rejected)
        {
            throw new InvalidOperationException("submissionId: Submission has already been rejected.");
        }

        return submission;
    }
}
