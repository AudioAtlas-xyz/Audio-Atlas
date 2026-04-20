namespace AudioAtlasDomain.MusicMetadata;

public class InstrumentDTO
{
    public Guid Id { get; set; }
    public string Type { get; set; } = null!;
    public string? Description { get; set; }
}
