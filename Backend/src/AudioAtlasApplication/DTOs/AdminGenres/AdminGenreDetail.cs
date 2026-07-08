namespace AudioAtlasApplication.DTOs.AdminGenres;

public sealed class AdminGenreDetail
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? StartYear { get; set; }
    public bool IsSensitive { get; set; }
    public string? SensitiveDescription { get; set; }
    public string? PlaylistLink { get; set; }
    public IReadOnlyList<LookupItem> Countries { get; set; } = [];
    public IReadOnlyList<LookupItem> Instruments { get; set; } = [];
    public IReadOnlyList<LookupItem> SimilarGenres { get; set; } = [];
    public IReadOnlyList<LookupItem> SubGenres { get; set; } = [];
    public IReadOnlyList<LookupItem> ParentGenres { get; set; } = [];
    public IReadOnlyList<string> Aliases { get; set; } = [];
    public IReadOnlyList<string> Sources { get; set; } = [];
    public bool IsArchived { get; set; }
    public DateTime? ArchivedAt { get; set; }
    public string? ArchivedByUsername { get; set; }
    public DateTime? LastEditedAt { get; set; }
    public string? LastEditedByUsername { get; set; }
    public string RowVersion { get; set; } = string.Empty;
}

public sealed class LookupItem
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
