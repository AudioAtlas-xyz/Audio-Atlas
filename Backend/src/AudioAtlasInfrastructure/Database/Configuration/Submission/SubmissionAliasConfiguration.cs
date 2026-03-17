using AudioAtlasDomain.Submissions;
using AudioAtlasDomain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AudioAtlasInfrastructure.Database.Configuration.Submission;

/// <summary>
/// Configures the SubmissionAlias entity for persistence.
/// 
/// This configuration models aliases as value-like entities within a submission,
/// where identity is defined by the combination of SubmissionId and Alias.
/// 
/// It ensures that each proposed alias is unique within a single submission,
/// while allowing the same alias to appear across different submissions.
/// </summary>
public class SubmissionAliasConfiguration : IEntityTypeConfiguration<SubmissionAlias>
{
    /// <summary>
    /// Configures the database schema and relationships for SubmissionAlias.
    /// </summary>
    public void Configure(EntityTypeBuilder<SubmissionAlias> builder)
    {
        /// <summary>
        /// Defines a composite primary key using SubmissionId and Alias.
        /// 
        /// This enforces that:
        /// - A proposed alias cannot be duplicated within the same submission
        /// - The alias is identified by its context, not independently
        /// </summary>
        builder.HasKey(x => new { x.SubmissionId, x.Alias });

        /// <summary>
        /// Configures the relationship between SubmissionAlias and Submission.
        /// 
        /// Each alias belongs to exactly one submission,
        /// and a submission can contain multiple proposed aliases.
        /// 
        /// No explicit delete behavior is defined,
        /// so EF Core will apply the default (typically cascade).
        /// </summary>
        builder.HasOne(x => x.Submission)
            .WithMany(x => x.Aliases)
            .HasForeignKey(x => x.SubmissionId);
    }
}
