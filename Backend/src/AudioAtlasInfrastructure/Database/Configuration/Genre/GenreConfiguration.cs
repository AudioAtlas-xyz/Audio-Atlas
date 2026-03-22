using AudioAtlasDomain.Genres;
using AudioAtlasDomain.Geography;
using AudioAtlasDomain.MusicMetadata;
using AudioAtlasDomain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace AudioAtlasInfrastructure.Database.Configuration.Genre;

/// <summary>
/// Configures the Genre entity, including its properties,
/// relationships, and join tables.
/// 
/// This configuration defines how genres connect to users,
/// metadata, geography, and other genres, forming a graph-like structure.
/// </summary>
public class GenreConfiguration : IEntityTypeConfiguration<AudioAtlasDomain.Genres.Genre>
{
    /// <summary>
    /// Configures the database schema and relationships for Genre.
    /// </summary>
    public void Configure(EntityTypeBuilder<AudioAtlasDomain.Genres.Genre> builder)
    {
        /// <summary>
        /// Maps the entity to the "Genres" table.
        /// </summary>
        builder.ToTable("Genres");

        /// <summary>
        /// Defines the primary key.
        /// </summary>
        builder.HasKey(x => x.Id);

        /// <summary>
        /// Configures required and optional string properties with length limits.
        /// </summary>
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasMaxLength(4000);

        builder.Property(x => x.PlaylistLink)
            .HasMaxLength(1000);

        builder.Property(x => x.SensitiveDescription)
            .HasMaxLength(4000);

        /// <summary>
        /// Configures the optional relationship to the author.
        /// 
        /// Restrict delete prevents accidental removal of genres
        /// when a user is deleted.
        /// </summary>
        builder.HasOne(x => x.Author)
            .WithMany()
            .HasForeignKey(x => x.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        /// <summary>
        /// Configures one-to-many relationships for aliases and sources.
        /// 
        /// Cascade delete ensures dependent data is removed
        /// when a genre is deleted.
        /// </summary>
        builder.HasMany(x => x.Aliases)
            .WithOne(x => x.Genre)
            .HasForeignKey(x => x.GenreId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Sources)
            .WithOne(x => x.Genre)
            .HasForeignKey(x => x.GenreId)
            .OnDelete(DeleteBehavior.Cascade);

        /// <summary>
        /// Configures many-to-many relationship between genres and countries.
        /// 
        /// Represents cultural or geographical association.
        /// Restrict delete prevents accidental removal of countries.
        /// </summary>
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
                    .HasOne<AudioAtlasDomain.Genres.Genre>()
                    .WithMany()
                    .HasForeignKey("GenreId")
                    .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasKey("GenreId", "CountryId");
                });

        /// <summary>
        /// Configures many-to-many relationship for user favorites.
        /// 
        /// Represents user preference rather than domain knowledge.
        /// </summary>
        builder.HasMany(x => x.Favoritees)
            .WithMany(x => x.FavoriteGenres)
            .UsingEntity<Dictionary<string, object>>(
                "FavoriteGenre",
                j => j
                    .HasOne<ApplicationUser>()
                    .WithMany()
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade),
                j => j
                    .HasOne<AudioAtlasDomain.Genres.Genre>()
                    .WithMany()
                    .HasForeignKey("GenreId")
                    .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasKey("UserId", "GenreId");
                });

        /// <summary>
        /// Configures many-to-many relationship between genres and instruments.
        /// 
        /// Represents typical instrumentation associated with a genre.
        /// </summary>
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
                .HasOne<AudioAtlasDomain.Genres.Genre>()
                .WithMany()
                .HasForeignKey("GenreId")
                .OnDelete(DeleteBehavior.Cascade),
            j =>
            {
                j.HasKey("GenreId", "InstrumentId");
            });

        /// <summary>
        /// Configures hierarchical relationships between genres.
        /// 
        /// Allows genres to have multiple parents and subgenres,
        /// forming a directed graph rather than a strict tree.
        /// 
        /// Restrict delete prevents cascading cycles in self-referencing data.
        /// </summary>
        builder.HasMany(x => x.SubGenres)
            .WithMany(x => x.ParentGenres)
            .UsingEntity<Dictionary<string, object>>(
                "GenreHierarchy",
                j => j
                    .HasOne<AudioAtlasDomain.Genres.Genre>()
                    .WithMany()
                    .HasForeignKey("SubGenreId")
                    .OnDelete(DeleteBehavior.Restrict),
                j => j
                    .HasOne<AudioAtlasDomain.Genres.Genre>()
                    .WithMany()
                    .HasForeignKey("ParentGenreId")
                    .OnDelete(DeleteBehavior.Restrict),
                j =>
                {
                    j.HasKey("ParentGenreId", "SubGenreId");
                });

        /// <summary>
        /// Configures similarity relationships between genres.
        /// 
        /// Represents non-hierarchical, stylistic connections.
        /// This relationship is directional unless enforced otherwise.
        /// </summary>
        builder.HasMany(x => x.SimilarGenres)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "GenreSimilarity",
                j => j
                    .HasOne<AudioAtlasDomain.Genres.Genre>()
                    .WithMany()
                    .HasForeignKey("SimilarGenreId")
                    .OnDelete(DeleteBehavior.Restrict),
                j => j
                    .HasOne<AudioAtlasDomain.Genres.Genre>()
                    .WithMany()
                    .HasForeignKey("GenreId")
                    .OnDelete(DeleteBehavior.Restrict),
                j =>
                {
                    j.HasKey("GenreId", "SimilarGenreId");
                });
        
        
        
    }
}
