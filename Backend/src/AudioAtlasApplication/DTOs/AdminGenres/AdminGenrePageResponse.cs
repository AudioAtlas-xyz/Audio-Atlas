namespace AudioAtlasApplication.DTOs.AdminGenres;

public sealed class AdminGenrePageResponse
{
    public IReadOnlyList<AdminGenreRow> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
