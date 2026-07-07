namespace AudioAtlasApplication.DTOs;

public class UpdateSubmissionRequest
{
    public string NewGenreName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsSensitive { get; set; }
    public string? SensitiveDescription { get; set; }
    public string? PlaylistLink { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public List<string> Aliases { get; set; } = [];
    public List<string> SourceLinks { get; set; } = [];
    public List<Guid> CountryIds { get; set; } = [];
    public List<Guid> InstrumentIds { get; set; } = [];
    public List<Guid> SimilarGenreIds { get; set; } = [];
    public List<Guid> SubGenreIds { get; set; } = [];
    public List<Guid> PredecessorGenreIds { get; set; } = [];
}
