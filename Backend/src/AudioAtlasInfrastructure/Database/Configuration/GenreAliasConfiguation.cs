using AudioAtlasDomain.Genres;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AudioAtlasInfrastructure.Database.Configuration;

/// <summary>
/// Configures the GenreAlias entity for persistence.
/// 
/// This configuration enforces uniqueness and integrity of aliases,
/// ensuring that each alias is distinct within the context of a single genre.
/// 
/// It also defines the relationship between GenreAlias and Genre,
/// including cascade delete behavior.
/// </summary>
public class GenreAliasConfiguration : IEntityTypeConfiguration<GenreAlias>
{
    /// <summary>
    /// Configures the database schema and relationships for GenreAlias.
    /// </summary>
    public void Configure(EntityTypeBuilder<GenreAlias> builder)
    {
        /// <summary>
        /// Defines the primary key for the entity.
        /// </summary>
        builder.HasKey(x => x.Id);

        /// <summary>
        /// Configures the Alias property as required with a maximum length.
        /// 
        /// This prevents excessively long or null alias values.
        /// </summary>
        builder.Property(x => x.Alias)
            .IsRequired()
            .HasMaxLength(255);

        /// <summary>
        /// Enforces uniqueness of alias values per genre.
        /// 
        /// Prevents duplicate aliases for the same genre,
        /// while allowing the same alias to exist across different genres.
        /// </summary>
        builder
            .HasIndex(x => new { x.GenreId, x.Alias })
            .IsUnique();

        /// <summary>
        /// Configures the relationship between GenreAlias and Genre.
        /// 
        /// Each alias belongs to exactly one genre,
        /// and a genre can have multiple aliases.
        /// 
        /// Cascade delete ensures that aliases are removed
        /// when the associated genre is deleted.
        /// </summary>
        builder
            .HasOne(x => x.Genre)
            .WithMany(g => g.Aliases)
            .HasForeignKey(x => x.GenreId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}