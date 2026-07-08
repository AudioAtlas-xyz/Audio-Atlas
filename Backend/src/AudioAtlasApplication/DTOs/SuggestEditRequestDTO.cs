namespace AudioAtlasApplication.DTOs;

public class SuggestEditRequest
{
    public string? Description { get; set; }
    public bool IsSensitive { get; set; }
    public string? SensitiveDescription { get; set; }
    public string? PlaylistLink { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public ICollection<string> Aliases { get; set; } = new List<string>();
    public ICollection<string> SourceLinks { get; set; } = new List<string>();
    public ICollection<Guid> CountryIds { get; set; } = new List<Guid>();
    public ICollection<Guid> InstrumentIds { get; set; } = new List<Guid>();
    public ICollection<Guid> SimilarGenreIds { get; set; } = new List<Guid>();
    public ICollection<Guid> SubGenreIds { get; set; } = new List<Guid>();
    public ICollection<Guid> PredecessorGenreIds { get; set; } = new List<Guid>();
}
