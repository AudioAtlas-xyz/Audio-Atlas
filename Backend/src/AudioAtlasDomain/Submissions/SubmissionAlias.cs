namespace AudioAtlasDomain.Submissions;

public class SubmissionAlias
{
    public Guid SubmissionId { get; set; }

    public string Alias { get; set; } = null!;

    public Submission Submission { get; set; } = null!;
}