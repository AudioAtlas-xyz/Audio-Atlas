namespace AudioAtlasDomain.Submissions
{
    /// <summary>
    /// Represents a source provided as part of a submission.
    /// 
    /// Submission sources support the claims made in a submission,
    /// but are not yet validated or accepted into the canonical data model.
    /// 
    /// They provide context, justification, and traceability during
    /// the review and moderation process.
    /// </summary>
    public class SubmissionSource
    {
        /// <summary>
        /// Identifier of the associated submission.
        /// </summary>
        public Guid SubmissionId { get; set; }

        /// <summary>
        /// URL or reference to the external source.
        /// 
        /// May include articles, databases, playlists, or other material
        /// used by the submitter to support their proposal.
        /// 
        /// The validity and reliability of the source are not guaranteed
        /// until reviewed.
        /// </summary>
        public string SourceLink { get; set; } = null!;

        /// <summary>
        /// Navigation property to the submission this source belongs to.
        /// 
        /// Multiple sources can be provided for a single submission
        /// to strengthen or contextualize its claims.
        /// </summary>
        public Submission Submission { get; set; } = null!;
    }
}
