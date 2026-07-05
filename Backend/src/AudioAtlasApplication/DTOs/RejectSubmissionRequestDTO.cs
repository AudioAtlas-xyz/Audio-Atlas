namespace AudioAtlasApplication.DTOs;

public class RejectSubmissionRequest
{
    public string RejectionReasonCode { get; set; } = string.Empty;
    public string? Note { get; set; }
}
