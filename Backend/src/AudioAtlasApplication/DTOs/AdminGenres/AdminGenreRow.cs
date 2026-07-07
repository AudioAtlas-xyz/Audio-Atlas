namespace AudioAtlasApplication.DTOs.AdminGenres;

public sealed class AdminGenreRow
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool HasDescription { get; set; }
    public bool HasSummary { get; set; }
    public IReadOnlyList<string> CountryNames { get; set; } = [];
    public bool IsArchived { get; set; }
    public DateTime? ArchivedAt { get; set; }
    public DateTime? LastEditedAt { get; set; }
    public string RowVersion { get; set; } = string.Empty;
}
