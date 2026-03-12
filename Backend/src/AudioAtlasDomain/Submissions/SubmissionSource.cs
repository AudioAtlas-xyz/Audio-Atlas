namespace AudioAtlasDomain.Submissions
{
    public class SubmissionSource
    {
        public Guid Id { get; set; }
        public Guid SubmissionId { get; set; }

        public string SourceLink { get; set; } = string.Empty;

        public Submission Submission { get; set; } = null!;
    }
}
