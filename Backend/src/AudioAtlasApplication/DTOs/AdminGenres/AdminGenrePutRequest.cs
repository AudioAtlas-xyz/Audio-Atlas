namespace AudioAtlasApplication.DTOs.AdminGenres;

public sealed class AdminGenrePutRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Summary { get; set; }
    public int? StartYear { get; set; }
    public bool IsSensitive { get; set; }
    public string? SensitiveDescription { get; set; }
    public string? PlaylistLink { get; set; }
    public List<Guid> CountryIds { get; set; } = [];
    public List<Guid> InstrumentIds { get; set; } = [];
    public List<Guid> SimilarGenreIds { get; set; } = [];
    public List<Guid> SubGenreIds { get; set; } = [];
    public List<Guid> ParentGenreIds { get; set; } = [];
    public List<string> Aliases { get; set; } = [];
    public List<string> Sources { get; set; } = [];
}
