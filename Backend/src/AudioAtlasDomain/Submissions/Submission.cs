using AudioAtlasDomain.Genres;
using AudioAtlasDomain.Geography;

namespace AudioAtlasDomain.Submissions;

public class Submission
{
    public Guid Id { get; set; }

    public string AccountId { get; set; } = string.Empty;

    public Guid? ExistingGenreId { get; set; }

    public string? NewGenreName { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string? Description { get; set; }

    public bool IsSensitive { get; set; }

    public string? PlaylistLink { get; set; }

    public bool IsRejected { get; set; }

    public ICollection<SubmissionAlias> Aliases { get; set; } = new List<SubmissionAlias>();

    public ICollection<SubmissionSource> Sources { get; set; } = new List<SubmissionSource>();

    public ICollection<Country> Countries { get; set; } = new List<Country>();

    public ICollection<Genre> SimilarGenres { get; set; } = new List<Genre>();

    public ICollection<Genre> SubGenres { get; set; } = new List<Genre>();

    public ICollection<Genre> PredecessorGenres { get; set; } = new List<Genre>();

    public RejectedSubmission? RejectedSubmission { get; set; }
}