using AudioAtlasDomain.Genres;

namespace AudioAtlasDomain.Geography;

public class CountryDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? Region { get; set; }
    public string? Continent { get; set; }
    public string? IsoCode { get; set; }
    public ICollection<GenreDTO> Genres { get; set; } = new List<GenreDTO>();
}
