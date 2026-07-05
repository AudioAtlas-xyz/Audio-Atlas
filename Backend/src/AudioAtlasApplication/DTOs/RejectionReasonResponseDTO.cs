namespace AudioAtlasApplication.DTOs;

public class RejectionReasonResponse
{
    public string Code { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string? GuidelineRef { get; set; }
}
