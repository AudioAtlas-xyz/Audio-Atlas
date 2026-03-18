using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AudioAtlasDomain.Submissions
{
    public class RejectedSubmission
    {
        public Guid SubmissionId { get; set; }

        public string Description { get; set; } = string.Empty;

        public Submission Submission { get; set; } = null!;
    }
}
