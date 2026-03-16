using System.ComponentModel.DataAnnotations;

namespace AudioAtlasDomain.Submissions
{
    public class RejectedSubmission
    {
        [Key]
        public Guid SubmissionId { get; set; }

        public string Description { get; set; } = string.Empty;

        public Submission Submission { get; set; } = null!;
    }
}
