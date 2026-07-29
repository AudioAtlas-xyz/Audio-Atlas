namespace AudioAtlasApplication.DTOs;

public class PendingSubmissionResponse
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public string? AccountUsername { get; set; }
    public string? NewGenreName { get; set; }
    public DateTime SubmittedAt { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? Description { get; set; }
    public bool IsSensitive { get; set; }
    public string? SensitiveDescription { get; set; }
    public string? ExampleSongYoutubeId { get; set; }
    public ICollection<string> Aliases { get; set; } = new List<string>();
    public ICollection<string> SourceLinks { get; set; } = new List<string>();
    public ICollection<Guid> CountryIds { get; set; } = new List<Guid>();
    public ICollection<Guid> InstrumentIds { get; set; } = new List<Guid>();
    public ICollection<Guid> SimilarGenreIds { get; set; } = new List<Guid>();
    public ICollection<Guid> SubGenreIds { get; set; } = new List<Guid>();
    public ICollection<Guid> PredecessorGenreIds { get; set; } = new List<Guid>();
    public Guid? TargetGenreId { get; set; }
    public string? TargetGenreName { get; set; }
    public bool IsEditSuggestion => TargetGenreId.HasValue;
}
