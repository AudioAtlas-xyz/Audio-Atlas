using AudioAtlasDomain.Submissions;
using AudioAtlasDomain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AudioAtlasInfrastructure.Database.Configuration.Submission;

public class SubmissionAliasConfiguration : IEntityTypeConfiguration<SubmissionAlias>
{
    public void Configure(EntityTypeBuilder<SubmissionAlias> builder)
    {
        builder.HasKey(x => new { x.SubmissionId, x.Alias });

        builder.HasOne(x => x.Submission)
            .WithMany(x => x.Aliases)
            .HasForeignKey(x => x.SubmissionId);
    }
}
