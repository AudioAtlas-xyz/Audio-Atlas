using AudioAtlasDomain.Enums;
using AudioAtlasDomain.Genres;
using AudioAtlasDomain.Geography;
using AudioAtlasDomain.Users;
using AudioAtlasDomain.MusicMetadata;

namespace AudioAtlasDomain.Submissions;

/// <summary>
/// Represents a user-submitted proposal for creating or extending genre data.
/// 
/// A submission is not considered authoritative data. Instead, it acts as an
/// intermediate structure awaiting validation, moderation, or rejection.
/// 
/// Submissions may propose new genres or contribute additional metadata
/// to existing ones, including relationships, context, and supporting sources.
/// </summary>
public class Submission
{
    /// <summary>
    /// Unique identifier for the submission.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Identifier of the user who created the submission.
    /// </summary>
    public Guid AccountId { get; set; }

    /// <summary>
    /// Proposed name for a new genre.
    /// 
    /// If null, the submission is assumed to modify or extend
    /// existing genre data rather than creating a new one.
    /// </summary>
    public string? NewGenreName { get; set; }

    /// <summary>
    /// Proposed start date of the genre or its active period.
    /// 
    /// Represents an approximate timeframe rather than an exact boundary.
    /// </summary>
    public DateOnly? StartDate { get; set; }

    /// <summary>
    /// Proposed end date of the genre or its active period.
    /// 
    /// May be null for ongoing or evolving genres.
    /// </summary>
    public DateOnly? EndDate { get; set; }

    /// <summary>
    /// Description of the proposed genre or changes.
    /// 
    /// Should explain defining characteristics, context,
    /// or reasoning behind the submission.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Indicates whether the submission contains or describes
    /// sensitive content.
    /// </summary>
    public bool IsSensitive { get; set; }

    /// <summary>
    /// SensitiveDescription
    /// </summary>
    public string? SensitiveDescription { get; set; }

    /// <summary>
    /// External reference (e.g., playlist) illustrating the submission.
    /// 
    /// Provides an experiential example of the proposed genre or changes.
    /// </summary>
    public string? PlaylistLink { get; set; }

    /// <summary>
    /// UTC timestamp of when the submission was created.
    /// </summary>
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Represents the current moderation state of the submission.
    /// </summary>
    public SubmissionStatus Status { get; set; } = SubmissionStatus.Pending;

    /// <summary>
    /// Proposed alternative names for the genre.
    /// </summary>
    public ICollection<SubmissionAlias> Aliases { get; set; } = new List<SubmissionAlias>();

    /// <summary>
    /// Sources supporting the submission.
    /// 
    /// Used to justify claims and provide traceability.
    /// </summary>
    public ICollection<SubmissionSource> Sources { get; set; } = new List<SubmissionSource>();

    /// <summary>
    /// Countries associated with the submission.
    /// 
    /// Reflects cultural or geographical context rather than strict origin.
    /// </summary>
    public ICollection<Country> Countries { get; set; } = new List<Country>();


    /// <summary>
    /// Instruments
    /// </summary>
    /// 
    public ICollection<Instrument> Instruments { get; set; } = new List<Instrument>();

    /// <summary>
    /// Genres considered stylistically similar in the context of this submission.
    /// 
    /// Used to position the proposed genre within the broader genre landscape.
    /// </summary>
    /// 
    public ICollection<Genre> SimilarGenres { get; set; } = new List<Genre>();

    /// <summary>
    /// Genres proposed as subgenres of the new or modified genre.
    /// </summary>
    public ICollection<Genre> SubGenres { get; set; } = new List<Genre>();

    /// <summary>
    /// Genres that precede or influence the proposed genre.
    /// 
    /// Represents lineage or evolutionary relationships.
    /// </summary>
    public ICollection<Genre> PredecessorGenres { get; set; } = new List<Genre>();

    /// <summary>
    /// UTC timestamp of when a curator completed their review.
    /// Null while still pending.
    /// </summary>
    public DateTime? ReviewedAt { get; set; }

    /// <summary>
    /// FK to the curator who completed the review.
    /// Null while pending; set to null if the reviewer's account is later deleted.
    /// </summary>
    public Guid? ReviewedById { get; set; }

    /// <summary>
    /// Navigation property to the reviewing curator.
    /// </summary>
    public ApplicationUser? ReviewedBy { get; set; }

    /// <summary>
    /// Structured rejection reason code from the RejectionReasons lookup table.
    /// Null unless the submission has been rejected.
    /// </summary>
    public string? RejectionReasonCode { get; set; }

    /// <summary>
    /// Navigation property to the structured rejection reason.
    /// </summary>
    public RejectionReason? RejectionReason { get; set; }

    /// <summary>
    /// Associated rejection record, if the submission has been rejected.
    ///
    /// RejectedSubmission.Description is the free-text curator note.
    /// The canonical machine-readable reason lives on RejectionReasonCode above.
    /// </summary>
    public RejectedSubmission? RejectedSubmission { get; set; }

    /// <summary>
    /// Navigation property to the submitting user.
    /// </summary>
    public ApplicationUser Account { get; set; } = null!;

    /// <summary>
    /// When set, this submission is an edit suggestion for an existing genre
    /// rather than a proposal for a new one.
    /// Null means this is a new-genre proposal.
    /// </summary>
    public Guid? TargetGenreId { get; set; }

    /// <summary>
    /// Navigation property to the genre being suggested for editing.
    /// </summary>
    public Genre? TargetGenre { get; set; }
}
