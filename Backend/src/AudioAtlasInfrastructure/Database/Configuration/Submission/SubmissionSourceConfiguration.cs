using AudioAtlasDomain.Submissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AudioAtlasInfrastructure.Database.Configuration.Submission;

public class SubmissionSourceConfiguration : IEntityTypeConfiguration<SubmissionSource>
{
    public void Configure(EntityTypeBuilder<SubmissionSource> builder)
    {
        builder.HasKey(x => new { x.SubmissionId, x.SourceLink });

        builder.HasOne(x => x.Submission)
            .WithMany(x => x.Sources)
            .HasForeignKey(x => x.SubmissionId);
    }
}
