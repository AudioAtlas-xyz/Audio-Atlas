using AudioAtlasDomain.Genres;
using AudioAtlasDomain.Geography;
using AudioAtlasDomain.MusicMetadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace AudioAtlasInfrastructure.Database.Configurations;

public class GenreConfiguration : IEntityTypeConfiguration<Genre>
{
    public void Configure(EntityTypeBuilder<Genre> builder)
    {
        builder.ToTable("Genres");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasMaxLength(4000);

        builder.Property(x => x.PlaylistLink)
            .HasMaxLength(1000);

        builder.Property(x => x.SensitiveDescription)
            .HasMaxLength(4000);

        builder.HasMany(x => x.Aliases)
            .WithOne(x => x.Genre)
            .HasForeignKey(x => x.GenreId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Sources)
            .WithOne(x => x.Genre)
            .HasForeignKey(x => x.GenreId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Countries)
            .WithMany(x => x.Genres)
            .UsingEntity<Dictionary<string, object>>(
                "GenreCountry",
                j => j
                    .HasOne<Country>()
                    .WithMany()
                    .HasForeignKey("CountryId")
                    .OnDelete(DeleteBehavior.Restrict),
                j => j
                    .HasOne<Genre>()
                    .WithMany()
                    .HasForeignKey("GenreId")
                    .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasKey("GenreId", "CountryId");
                });

        builder.HasMany(x => x.Instruments)
        .WithMany(x => x.Genres)
        .UsingEntity<Dictionary<string, object>>(
            "GenreInstrument",
            j => j
                .HasOne<Instrument>()
                .WithMany()
                .HasForeignKey("InstrumentId")
                .OnDelete(DeleteBehavior.Restrict),
            j => j
                .HasOne<Genre>()
                .WithMany()
                .HasForeignKey("GenreId")
                .OnDelete(DeleteBehavior.Cascade),
            j =>
            {
                j.HasKey("GenreId", "InstrumentId");
            });

        // Hierarchy: ParentGenre -> SubGenre
        builder.HasMany(x => x.SubGenres)
            .WithMany(x => x.ParentGenres)
            .UsingEntity<Dictionary<string, object>>(
                "GenreHierarchy",
                j => j
                    .HasOne<Genre>()
                    .WithMany()
                    .HasForeignKey("SubGenreId")
                    .OnDelete(DeleteBehavior.Restrict),
                j => j
                    .HasOne<Genre>()
                    .WithMany()
                    .HasForeignKey("ParentGenreId")
                    .OnDelete(DeleteBehavior.Restrict),
                j =>
                {
                    j.HasKey("ParentGenreId", "SubGenreId");
                });

        // Similarity: Genre <-> SimilarGenre
        builder.HasMany(x => x.SimilarGenres)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "GenreSimilarity",
                j => j
                    .HasOne<Genre>()
                    .WithMany()
                    .HasForeignKey("SimilarGenreId")
                    .OnDelete(DeleteBehavior.Restrict),
                j => j
                    .HasOne<Genre>()
                    .WithMany()
                    .HasForeignKey("GenreId")
                    .OnDelete(DeleteBehavior.Restrict),
                j =>
                {
                    j.HasKey("GenreId", "SimilarGenreId");
                });
    }
}