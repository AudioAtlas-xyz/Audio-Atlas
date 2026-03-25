using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AudioAtlasDomain.Submissions
{
    /// <summary>
    /// Represents a submission that has been reviewed and rejected.
    /// 
    /// This entity stores the outcome of a moderation decision,
    /// including the reason for rejection. It preserves context
    /// for transparency, auditing, and potential future review.
    /// </summary>
    public class RejectedSubmission
    {
        /// <summary>
        /// Identifier of the associated submission.
        /// 
        /// Acts as both the primary key and a foreign key
        /// to the original submission.
        /// </summary>
        public Guid SubmissionId { get; set; }

        /// <summary>
        /// Explanation for why the submission was rejected.
        /// 
        /// Should provide clear, actionable feedback rather than
        /// a simple label, enabling users or moderators to understand
        /// the reasoning behind the decision.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Navigation property to the rejected submission.
        /// 
        /// Establishes a one-to-one relationship where each rejected
        /// submission has exactly one corresponding rejection record.
        /// </summary>
        public Submission Submission { get; set; } = null!;
    }
}
