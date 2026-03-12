namespace AudioAtlasDomain.Submissions
{
    public class SubmissionCountry
    {
        public Guid SubmissionId { get; set; }

        public int CountryId { get; set; }

        public Submission Submission { get; set; } = null!;
    }
}
