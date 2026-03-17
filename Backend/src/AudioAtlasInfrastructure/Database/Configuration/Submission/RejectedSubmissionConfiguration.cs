using AudioAtlasDomain.Submissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AudioAtlasInfrastructure.Database.Configuration.Submission;

/// <summary>
/// Configures the RejectedSubmission entity for persistence.
/// 
/// This configuration models a rejection as a one-to-one extension
/// of a submission, where the rejection record provides context
/// and reasoning for the decision.
/// 
/// It ensures that each rejected submission has exactly one
/// corresponding rejection record.
/// </summary>
public class RejectedSubmissionConfiguration : IEntityTypeConfiguration<RejectedSubmission>
{
    /// <summary>
    /// Configures the database schema and relationships for RejectedSubmission.
    /// </summary>
    public void Configure(EntityTypeBuilder<RejectedSubmission> builder)
    {
        /// <summary>
        /// Uses SubmissionId as both primary key and foreign key.
        /// 
        /// This enforces a strict one-to-one relationship where
        /// the rejection record cannot exist without its submission.
        /// </summary>
        builder.HasKey(x => x.SubmissionId);

        /// <summary>
        /// Configures the rejection description as required
        /// with a maximum length constraint.
        /// 
        /// Ensures that every rejection includes an explanation.
        /// </summary>
        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(2000);

        /// <summary>
        /// Configures the one-to-one relationship between
        /// Submission and RejectedSubmission.
        /// 
        /// Cascade delete ensures that if a submission is removed,
        /// its associated rejection record is also deleted.
        /// </summary>
        builder.HasOne(x => x.Submission)
            .WithOne(x => x.RejectedSubmission)
            .HasForeignKey<RejectedSubmission>(x => x.SubmissionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
