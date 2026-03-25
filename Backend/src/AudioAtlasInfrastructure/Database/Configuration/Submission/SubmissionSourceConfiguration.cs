using AudioAtlasDomain.Submissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AudioAtlasInfrastructure.Database.Configuration.Submission;

/// <summary>
/// Configures the SubmissionSource entity for persistence.
/// 
/// This configuration models sources as value-like entities within a submission,
/// where identity is defined by the combination of SubmissionId and SourceLink.
/// 
/// It ensures that each source is unique within a single submission,
/// while allowing reuse of the same source across different submissions.
/// </summary>
public class SubmissionSourceConfiguration : IEntityTypeConfiguration<SubmissionSource>
{
    /// <summary>
    /// Configures the database schema and relationships for SubmissionSource.
    /// </summary>
    public void Configure(EntityTypeBuilder<SubmissionSource> builder)
    {
        /// <summary>
        /// Defines a composite primary key using SubmissionId and SourceLink.
        /// 
        /// This enforces that:
        /// - A source cannot be duplicated within the same submission
        /// - The source is identified by its context, not independently
        /// </summary>
        builder.HasKey(x => new { x.SubmissionId, x.SourceLink });

        /// <summary>
        /// Configures the relationship between SubmissionSource and Submission.
        /// 
        /// Each source belongs to exactly one submission,
        /// and a submission can contain multiple supporting sources.
        /// </summary>
        builder.HasOne(x => x.Submission)
            .WithMany(x => x.Sources)
            .HasForeignKey(x => x.SubmissionId);
    }
}
