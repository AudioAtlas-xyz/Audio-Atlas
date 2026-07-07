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
    private const int MaxNoteLength = 2000;

    private readonly ICountryRepository _countryRepository;
    private readonly IGenreRepository _genreRepository;
    private readonly IInstrumentRepository _instrumentRepository;
    private readonly ISubmissionRepository _submissionRepository;

    public SubmissionService(
        ICountryRepository countryRepository,
        IGenreRepository genreRepository,
        IInstrumentRepository instrumentRepository,
        ISubmissionRepository submissionRepository)
    {
        _countryRepository = countryRepository;
        _genreRepository = genreRepository;
        _instrumentRepository = instrumentRepository;
        _submissionRepository = submissionRepository;
    }

    public async Task<Guid> createSubmissionAsync(Guid accountId, CreateSubmissionRequest request, CancellationToken cancellationToken = default)
    {
        var errors = new Dictionary<string, string[]>();

        var normalizedGenreName = normalizeText(request.NewGenreName);
        var normalizedDescription = normalizeText(request.Description);
        var normalizedPlaylistLink = normalizeOptionalUrl(request.PlaylistLink, "playlistLink", errors);
        var aliasValues = normalizeDistinctTexts(request.Aliases ?? []);
        var normalizedSourceLinks = normalizeDistinctUrls(request.SourceLinks ?? [], "sourceLinks", errors);
        var countryIds = distinctIds(request.CountryIds ?? []);
        var instrumentIds = distinctIds(request.InstrumentIds ?? []);
        var similarGenreIds = distinctIds(request.SimilarGenreIds ?? []);
        var subGenreIds = distinctIds(request.SubGenreIds ?? []);
        var predecessorGenreIds = distinctIds(request.PredecessorGenreIds ?? []);

        if (string.IsNullOrWhiteSpace(normalizedGenreName))
            errors["newGenreName"] = ["New genre name is required."];
        else if (normalizedGenreName.Length > MaxNameLength)
            errors["newGenreName"] = [$"New genre name must be at most {MaxNameLength} characters."];

        if (string.IsNullOrWhiteSpace(normalizedDescription))
            errors["description"] = ["Description is required."];
        else if (normalizedDescription.Length > MaxDescriptionLength)
            errors["description"] = [$"Description must be at most {MaxDescriptionLength} characters."];

        if (request.StartDate.HasValue && request.EndDate.HasValue && request.StartDate.Value > request.EndDate.Value)
            errors["dateRange"] = ["Start date must be earlier than or equal to end date."];

        if (aliasValues.Any(alias => alias.Length > MaxAliasLength))
            errors["aliases"] = [$"Aliases must be at most {MaxAliasLength} characters."];

        if (normalizedPlaylistLink is { Length: > MaxUrlLength })
            errors["playlistLink"] = [$"Playlist link must be at most {MaxUrlLength} characters."];

        if (normalizedSourceLinks.Any(link => link.Length > MaxUrlLength))
            errors["sourceLinks"] = [$"Source links must be at most {MaxUrlLength} characters."];

        var normalizedAliases = aliasValues
            .Where(alias => alias.Length <= MaxAliasLength)
            .ToList();

        if (errors.Count > 0)
            throw new InvalidOperationException(buildErrorMessage(errors));

        var countries = await _countryRepository.getCountriesByIdsAsync(countryIds, cancellationToken);
        var instruments = await _instrumentRepository.getInstrumentsByIdsAsync(instrumentIds, cancellationToken);
        var similarGenres = await _genreRepository.getGenresByIdsAsync(similarGenreIds, cancellationToken);
        var subGenres = await _genreRepository.getGenresByIdsAsync(subGenreIds, cancellationToken);
        var predecessorGenres = await _genreRepository.getGenresByIdsAsync(predecessorGenreIds, cancellationToken);

        addMissingIdErrors(errors, "countryIds", countryIds, countries.Select(country => country.Id));
        addMissingIdErrors(errors, "instrumentIds", instrumentIds, instruments.Select(instrument => instrument.Id));
        addMissingIdErrors(errors, "similarGenreIds", similarGenreIds, similarGenres.Select(genre => genre.Id));
        addMissingIdErrors(errors, "subGenreIds", subGenreIds, subGenres.Select(genre => genre.Id));
        addMissingIdErrors(errors, "predecessorGenreIds", predecessorGenreIds, predecessorGenres.Select(genre => genre.Id));

        if (errors.Count > 0)
            throw new InvalidOperationException(buildErrorMessage(errors));

        var submission = new Submission
        {
            AccountId = accountId,
            NewGenreName = normalizedGenreName,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Description = normalizedDescription,
            IsSensitive = request.IsSensitive,
            SensitiveDescription = request.IsSensitive ? normalizeText(request.SensitiveDescription) : null,
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
            Instruments = instruments.ToList(),
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
                SubmittedAt = submission.SubmittedAt,
                StartDate = submission.StartDate,
                EndDate = submission.EndDate,
                Description = submission.Description,
                IsSensitive = submission.IsSensitive,
                SensitiveDescription = submission.SensitiveDescription,
                PlaylistLink = submission.PlaylistLink,
                Aliases = submission.Aliases.Select(alias => alias.Alias).ToList(),
                SourceLinks = submission.Sources.Select(source => source.SourceLink).ToList(),
                CountryIds = submission.Countries.Select(country => country.Id).ToList(),
                InstrumentIds = submission.Instruments.Select(instrument => instrument.Id).ToList(),
                SimilarGenreIds = submission.SimilarGenres.Select(genre => genre.Id).ToList(),
                SubGenreIds = submission.SubGenres.Select(genre => genre.Id).ToList(),
                PredecessorGenreIds = submission.PredecessorGenres.Select(genre => genre.Id).ToList()
            })
            .ToList();
    }

    public async Task approveAsync(Guid submissionId, Guid reviewerId, CancellationToken cancellationToken = default)
    {
        var submission = await getReviewableSubmissionOrThrowAsync(submissionId, cancellationToken);

        var genre = new AudioAtlasDomain.Genres.Genre
        {
            AuthorId = submission.AccountId,
            Name = submission.NewGenreName!,
            Description = submission.Description,
            StartYear = submission.StartDate?.Year,
            IsSensitive = submission.IsSensitive,
            SensitiveDescription = submission.SensitiveDescription,
            PlaylistLink = submission.PlaylistLink,
            Aliases = submission.Aliases
                .Select(a => new AudioAtlasDomain.Genres.GenreAlias { Alias = a.Alias })
                .ToList(),
            Sources = submission.Sources
                .Select(s => new AudioAtlasDomain.Genres.GenreSource { SourceLink = s.SourceLink })
                .ToList(),
            Countries = submission.Countries.ToList(),
            Instruments = submission.Instruments.ToList(),
            SimilarGenres = submission.SimilarGenres.ToList(),
            SubGenres = submission.SubGenres.ToList(),
            ParentGenres = submission.PredecessorGenres.ToList(),
        };

        await _genreRepository.AddAsync(genre, cancellationToken);

        submission.Status = SubmissionStatus.Approved;
        submission.ReviewedAt = DateTime.UtcNow;
        submission.ReviewedById = reviewerId;
        submission.RejectedSubmission = null;

        await _submissionRepository.saveChangesAsync(cancellationToken);
    }

    public async Task rejectAsync(Guid submissionId, Guid reviewerId, RejectSubmissionRequest request, CancellationToken cancellationToken = default)
    {
        var submission = await getReviewableSubmissionOrThrowAsync(submissionId, cancellationToken);

        var normalizedCode = normalizeText(request.RejectionReasonCode);

        if (string.IsNullOrWhiteSpace(normalizedCode))
            throw new InvalidOperationException("rejectionReasonCode: A rejection reason code is required.");

        var reason = await _submissionRepository.getActiveRejectionReasonAsync(normalizedCode, cancellationToken);

        if (reason is null)
            throw new InvalidOperationException($"rejectionReasonCode: '{normalizedCode}' is not a recognised or active rejection reason.");

        var normalizedNote = normalizeText(request.Note);

        if (normalizedNote is { Length: > MaxNoteLength })
            throw new InvalidOperationException($"note: Note must be at most {MaxNoteLength} characters.");

        submission.Status = SubmissionStatus.Rejected;
        submission.ReviewedAt = DateTime.UtcNow;
        submission.ReviewedById = reviewerId;
        submission.RejectionReasonCode = normalizedCode;

        if (submission.RejectedSubmission is null)
        {
            submission.RejectedSubmission = new RejectedSubmission
            {
                SubmissionId = submission.Id,
                Description = normalizedNote ?? string.Empty
            };
        }
        else
        {
            submission.RejectedSubmission.Description = normalizedNote ?? string.Empty;
        }

        await _submissionRepository.saveChangesAsync(cancellationToken);
    }

    public async Task holdForSensitivityAsync(Guid submissionId, Guid reviewerId, CancellationToken cancellationToken = default)
    {
        var submission = await getPendingSubmissionOrThrowAsync(submissionId, cancellationToken);

        submission.Status = SubmissionStatus.OnHoldSensitivity;
        submission.ReviewedAt = DateTime.UtcNow;
        submission.ReviewedById = reviewerId;

        await _submissionRepository.saveChangesAsync(cancellationToken);
    }

    public async Task<ICollection<RejectionReasonResponse>> getActiveRejectionReasonsAsync(CancellationToken cancellationToken = default)
    {
        var reasons = await _submissionRepository.getActiveRejectionReasonsAsync(cancellationToken);

        return reasons
            .Select(r => new RejectionReasonResponse
            {
                Code = r.Code,
                Label = r.Label,
                GuidelineRef = r.GuidelineRef
            })
            .ToList();
    }

    public async Task updateSubmissionAsync(Guid submissionId, UpdateSubmissionRequest request, CancellationToken cancellationToken = default)
    {
        var submission = await getReviewableSubmissionOrThrowAsync(submissionId, cancellationToken);

        var errors = new Dictionary<string, string[]>();

        var normalizedGenreName = normalizeText(request.NewGenreName);
        var normalizedDescription = normalizeText(request.Description);
        var normalizedPlaylistLink = normalizeOptionalUrl(request.PlaylistLink, "playlistLink", errors);
        var aliasValues = normalizeDistinctTexts(request.Aliases ?? []);
        var normalizedSourceLinks = normalizeDistinctUrls(request.SourceLinks ?? [], "sourceLinks", errors);
        var countryIds = distinctIds(request.CountryIds ?? []);
        var instrumentIds = distinctIds(request.InstrumentIds ?? []);
        var similarGenreIds = distinctIds(request.SimilarGenreIds ?? []);
        var subGenreIds = distinctIds(request.SubGenreIds ?? []);
        var predecessorGenreIds = distinctIds(request.PredecessorGenreIds ?? []);

        if (string.IsNullOrWhiteSpace(normalizedGenreName))
            errors["newGenreName"] = ["New genre name is required."];
        else if (normalizedGenreName.Length > MaxNameLength)
            errors["newGenreName"] = [$"New genre name must be at most {MaxNameLength} characters."];

        if (string.IsNullOrWhiteSpace(normalizedDescription))
            errors["description"] = ["Description is required."];
        else if (normalizedDescription.Length > MaxDescriptionLength)
            errors["description"] = [$"Description must be at most {MaxDescriptionLength} characters."];

        if (request.StartDate.HasValue && request.EndDate.HasValue && request.StartDate.Value > request.EndDate.Value)
            errors["dateRange"] = ["Start date must be earlier than or equal to end date."];

        if (errors.Count > 0)
            throw new InvalidOperationException(buildErrorMessage(errors));

        var countries = await _countryRepository.getCountriesByIdsAsync(countryIds, cancellationToken);
        var instruments = await _instrumentRepository.getInstrumentsByIdsAsync(instrumentIds, cancellationToken);
        var similarGenres = await _genreRepository.getGenresByIdsAsync(similarGenreIds, cancellationToken);
        var subGenres = await _genreRepository.getGenresByIdsAsync(subGenreIds, cancellationToken);
        var predecessorGenres = await _genreRepository.getGenresByIdsAsync(predecessorGenreIds, cancellationToken);

        addMissingIdErrors(errors, "countryIds", countryIds, countries.Select(c => c.Id));
        addMissingIdErrors(errors, "instrumentIds", instrumentIds, instruments.Select(i => i.Id));
        addMissingIdErrors(errors, "similarGenreIds", similarGenreIds, similarGenres.Select(g => g.Id));
        addMissingIdErrors(errors, "subGenreIds", subGenreIds, subGenres.Select(g => g.Id));
        addMissingIdErrors(errors, "predecessorGenreIds", predecessorGenreIds, predecessorGenres.Select(g => g.Id));

        if (errors.Count > 0)
            throw new InvalidOperationException(buildErrorMessage(errors));

        submission.NewGenreName = normalizedGenreName;
        submission.Description = normalizedDescription;
        submission.IsSensitive = request.IsSensitive;
        submission.SensitiveDescription = request.IsSensitive ? normalizeText(request.SensitiveDescription) : null;
        submission.PlaylistLink = normalizedPlaylistLink;
        submission.StartDate = request.StartDate;
        submission.EndDate = request.EndDate;

        submission.Aliases.Clear();
        foreach (var alias in aliasValues.Where(a => a.Length <= MaxAliasLength))
            submission.Aliases.Add(new SubmissionAlias { Alias = alias });

        submission.Sources.Clear();
        foreach (var link in normalizedSourceLinks)
            submission.Sources.Add(new SubmissionSource { SourceLink = link });

        submission.Countries.Clear();
        foreach (var c in countries) submission.Countries.Add(c);

        submission.Instruments.Clear();
        foreach (var i in instruments) submission.Instruments.Add(i);

        submission.SimilarGenres.Clear();
        foreach (var g in similarGenres) submission.SimilarGenres.Add(g);

        submission.SubGenres.Clear();
        foreach (var g in subGenres) submission.SubGenres.Add(g);

        submission.PredecessorGenres.Clear();
        foreach (var g in predecessorGenres) submission.PredecessorGenres.Add(g);

        await _submissionRepository.saveChangesAsync(cancellationToken);
    }

    private static void addMissingIdErrors(
        Dictionary<string, string[]> errors,
        string key,
        IReadOnlyCollection<Guid> requestedIds,
        IEnumerable<Guid> resolvedIds)
    {
        var resolvedSet = resolvedIds.ToHashSet();
        var missingIds = requestedIds.Where(id => !resolvedSet.Contains(id)).ToArray();

        if (missingIds.Length > 0)
            errors[key] = [$"Unknown ids: {string.Join(", ", missingIds)}"];
    }

    private static string? normalizeText(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static string? normalizeOptionalUrl(string? value, string fieldName, Dictionary<string, string[]> errors)
    {
        var normalized = normalizeText(value);

        if (normalized is null)
            return null;

        if (!isHttpUrl(normalized))
            errors[fieldName] = ["Value must be an absolute http or https URL."];

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
            errors[fieldName] = ["All source links must be absolute http or https URLs."];

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

    // Allows Pending and OnHoldSensitivity — used by approve and reject.
    private async Task<Submission> getReviewableSubmissionOrThrowAsync(Guid submissionId, CancellationToken cancellationToken)
    {
        var submission = await _submissionRepository.getByIdAsync(submissionId, cancellationToken);

        if (submission is null)
            throw new InvalidOperationException("submissionId: Submission not found.");

        if (submission.Status == SubmissionStatus.Approved)
            throw new InvalidOperationException("submissionId: Submission has already been approved.");

        if (submission.Status == SubmissionStatus.Rejected)
            throw new InvalidOperationException("submissionId: Submission has already been rejected.");

        return submission;
    }

    // Only allows Pending — used by holdForSensitivityAsync.
    private async Task<Submission> getPendingSubmissionOrThrowAsync(Guid submissionId, CancellationToken cancellationToken)
    {
        var submission = await getReviewableSubmissionOrThrowAsync(submissionId, cancellationToken);

        if (submission.Status == SubmissionStatus.OnHoldSensitivity)
            throw new InvalidOperationException("submissionId: Submission is already on hold for sensitivity review.");

        return submission;
    }
}
