using System.ComponentModel.DataAnnotations;

namespace AudioAtlasDomain.Submissions
{
    public class SubmissionCountry
    {
        [Key]
        public Guid SubmissionId { get; set; }

        public int CountryId { get; set; }

        public Submission Submission { get; set; } = null!;
    }
}
