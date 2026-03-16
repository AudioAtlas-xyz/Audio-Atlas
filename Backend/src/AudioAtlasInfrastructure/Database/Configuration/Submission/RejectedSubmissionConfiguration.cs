using AudioAtlasDomain.Submissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AudioAtlasInfrastructure.Database.Configuration;

public class RejectedSubmissionConfiguration : IEntityTypeConfiguration<RejectedSubmission>
{
    public void Configure(EntityTypeBuilder<RejectedSubmission> builder)
    {
        builder.HasKey(x => x.SubmissionId);

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.HasOne(x => x.Submission)
            .WithOne(x => x.RejectedSubmission)
            .HasForeignKey<RejectedSubmission>(x => x.SubmissionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
