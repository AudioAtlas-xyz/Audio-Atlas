using AudioAtlasDomain.Geography;
using AudioAtlasDomain.Genres;
using AudioAtlasDomain.Submissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AudioAtlasInfrastructure.Database.Configuration.Submission;

public class SubmissionConfiguration : IEntityTypeConfiguration<AudioAtlasDomain.Submissions.Submission>
{
    public void Configure(EntityTypeBuilder<AudioAtlasDomain.Submissions.Submission> builder)
    {
        builder.ToTable("Submissions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.AccountId)
            .IsRequired();

        builder.HasOne(x => x.Account)
            .WithMany()
            .HasForeignKey(x => x.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Aliases)
            .WithOne(x => x.Submission)
            .HasForeignKey(x => x.SubmissionId);

        builder.HasMany(x => x.Sources)
            .WithOne(x => x.Submission)
            .HasForeignKey(x => x.SubmissionId);

        builder.HasMany(x => x.Countries)
            .WithMany(x => x.Submissions)
            .UsingEntity<Dictionary<string, object>>(
                "SubmissionCountry",
                j => j
                    .HasOne<Country>()
                    .WithMany()
                    .HasForeignKey("CountryId")
                    .OnDelete(DeleteBehavior.Restrict),
                j => j
                    .HasOne<AudioAtlasDomain.Submissions.Submission>()
                    .WithMany()
                    .HasForeignKey("SubmissionId")
                    .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasKey("SubmissionId", "CountryId");
                });

        builder.HasMany(x => x.SimilarGenres)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "SubmissionSimilarGenre",
                j => j
                    .HasOne<AudioAtlasDomain.Genres.Genre>()
                    .WithMany()
                    .HasForeignKey("GenreId")
                    .OnDelete(DeleteBehavior.Restrict),
                j => j
                    .HasOne<AudioAtlasDomain.Submissions.Submission>()
                    .WithMany()
                    .HasForeignKey("SubmissionId")
                    .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasKey("SubmissionId", "GenreId");
                });

        builder.HasMany(x => x.SubGenres)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "SubmissionSubGenre",
                j => j
                    .HasOne<AudioAtlasDomain.Genres.Genre>()
                    .WithMany()
                    .HasForeignKey("GenreId")
                    .OnDelete(DeleteBehavior.Restrict),
                j => j
                    .HasOne<AudioAtlasDomain.Submissions.Submission>()
                    .WithMany()
                    .HasForeignKey("SubmissionId")
                    .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasKey("SubmissionId", "GenreId");
                });

        builder.HasMany(x => x.PredecessorGenres)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "SubmissionPredecessorGenre",
                j => j
                    .HasOne<AudioAtlasDomain.Genres.Genre>()
                    .WithMany()
                    .HasForeignKey("GenreId")
                    .OnDelete(DeleteBehavior.Restrict),
                j => j
                    .HasOne<AudioAtlasDomain.Submissions.Submission>()
                    .WithMany()
                    .HasForeignKey("SubmissionId")
                    .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasKey("SubmissionId", "GenreId");
                });
    }
}
