using AudioAtlasDomain.Genres;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AudioAtlasInfrastructure.Database.Configuration.Genre
{
    /// <summary>
    /// Configures the GenreSource entity for persistence.
    /// 
    /// This configuration treats sources as value-like entities,
    /// where identity is defined by the combination of GenreId and SourceLink.
    /// 
    /// It ensures that the same source cannot be duplicated
    /// for a single genre, while allowing reuse across different genres.
    /// </summary>
    public class GenreSourceConfiguration : IEntityTypeConfiguration<GenreSource>
    {
        /// <summary>
        /// Configures the database schema and relationships for GenreSource.
        /// </summary>
        public void Configure(EntityTypeBuilder<GenreSource> builder)
        {
            /// <summary>
            /// Defines a composite primary key using GenreId and SourceLink.
            /// 
            /// This enforces that:
            /// - A source link is unique within the scope of a genre
            /// - The source is identified by its association, not independently
            /// </summary>
            builder.HasKey(x => new { x.GenreId, x.SourceLink });

            /// <summary>
            /// Configures the relationship between GenreSource and Genre.
            /// 
            /// Each source belongs to exactly one genre,
            /// and a genre can have multiple supporting sources.
            /// 
            /// No explicit delete behavior is defined,
            /// so EF Core will apply the default (typically cascade).
            /// </summary>
            builder.HasOne(x => x.Genre).WithMany(x => x.Sources);
        }




    }
}
