using AudioAtlasDomain.Geography;
using AudioAtlasDomain.Genres;
using AudioAtlasDomain.Submissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AudioAtlasInfrastructure.Database.Configuration.Submission;

/// <summary>
/// Configures the Submission entity, including its properties,
/// relationships, and join tables.
/// 
/// This configuration defines how user-submitted proposals connect
/// to existing domain entities such as genres and countries.
/// 
/// Submissions represent unvalidated data and should not be treated
/// as canonical until approved.
/// </summary>
public class SubmissionConfiguration : IEntityTypeConfiguration<AudioAtlasDomain.Submissions.Submission>
{
    /// <summary>
    /// Configures the database schema and relationships for Submission.
    /// </summary>
    public void Configure(EntityTypeBuilder<AudioAtlasDomain.Submissions.Submission> builder)
    {
        /// <summary>
        /// Maps the entity to the "Submissions" table.
        /// </summary>
        builder.ToTable("Submissions");

        /// <summary>
        /// Defines the primary key.
        /// </summary>
        builder.HasKey(x => x.Id);

        /// <summary>
        /// Configures the required relationship to the submitting user.
        /// 
        /// Restrict delete prevents accidental removal of submissions
        /// if a user is deleted.
        /// </summary>
        builder.Property(x => x.AccountId)
            .IsRequired();

        builder.HasOne(x => x.Account)
            .WithMany()
            .HasForeignKey(x => x.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        /// <summary>
        /// Configures one-to-many relationships for proposed aliases and sources.
        /// </summary>
        builder.HasMany(x => x.Aliases)
            .WithOne(x => x.Submission)
            .HasForeignKey(x => x.SubmissionId);

        builder.HasMany(x => x.Sources)
            .WithOne(x => x.Submission)
            .HasForeignKey(x => x.SubmissionId);

        /// <summary>
        /// Configures many-to-many relationship between submissions and countries.
        /// 
        /// Represents proposed geographical or cultural associations.
        /// </summary>
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

        /// <summary>
        /// Configures similarity relationships between submissions and existing genres.
        /// 
        /// Used to position the proposed genre within the current genre landscape.
        /// </summary>
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

        /// <summary>
        /// Configures proposed subgenre relationships.
        /// 
        /// Represents how the submitted genre may fit into
        /// an existing hierarchy as a more specific genre.
        /// </summary>
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

        /// <summary>
        /// Configures predecessor relationships.
        /// 
        /// Represents genres that influence or precede the proposed genre,
        /// forming a potential evolutionary lineage.
        /// </summary>
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
