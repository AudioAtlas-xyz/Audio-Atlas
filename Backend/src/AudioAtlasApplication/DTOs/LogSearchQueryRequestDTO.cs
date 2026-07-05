namespace AudioAtlasApplication.DTOs;

public class LogSearchQueryRequest
{
    public string Term { get; set; } = string.Empty;
    public int ResultCount { get; set; }
    public string? ContextRegion { get; set; }
    public string? ContextContinent { get; set; }
}
