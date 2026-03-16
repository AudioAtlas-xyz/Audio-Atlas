using AudioAtlasDomain.Submissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AudioAtlasInfrastructure.Database.Configuration;

public class SubmissionConfiguration : IEntityTypeConfiguration<Submission>
{
    public void Configure(EntityTypeBuilder<Submission> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.AccountId)
            .IsRequired();

        builder.HasMany(x => x.Aliases)
            .WithOne(x => x.Submission)
            .HasForeignKey(x => x.SubmissionId);

        builder.HasMany(x => x.Sources)
            .WithOne(x => x.Submission)
            .HasForeignKey(x => x.SubmissionId);

        builder.HasMany(x => x.SimilarGenres)
            .WithMany()
            .UsingEntity("SubmissionSimilarGenres");

        builder.HasMany(x => x.SubGenres)
            .WithMany()
            .UsingEntity("SubmissionSubGenres");

        builder.HasMany(x => x.PredecessorGenres)
            .WithMany()
            .UsingEntity("SubmissionPredecessorGenres");
    }
}
