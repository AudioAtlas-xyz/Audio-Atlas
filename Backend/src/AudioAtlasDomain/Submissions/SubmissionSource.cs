namespace AudioAtlasDomain.Submissions
{
    public class SubmissionSource
    {
        public Guid SubmissionId { get; set; }

        public string SourceLink { get; set; } = null!;

        public Submission Submission { get; set; } = null!;
    }
}
