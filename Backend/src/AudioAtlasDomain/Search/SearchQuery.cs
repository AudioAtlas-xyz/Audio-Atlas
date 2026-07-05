namespace AudioAtlasDomain.Search;

public class SearchQuery
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Term { get; set; } = string.Empty;
    public string NormalizedTerm { get; set; } = string.Empty;
    public int ResultCount { get; set; }
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    public string? ContextRegion { get; set; }
    public string? ContextContinent { get; set; }
}
