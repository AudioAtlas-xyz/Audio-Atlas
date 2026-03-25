using AudioAtlasDomain.Genres;
using AudioAtlasDomain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace AudioAtlasInfrastructure.Database.Configuration.Genre
{
    /// <summary>
    /// Configures the GenreAlias entity for persistence.
    /// 
    /// This configuration models aliases as value-like entities,
    /// where identity is defined by the combination of GenreId and Alias.
    /// 
    /// It ensures that each alias is unique within the scope of a genre,
    /// while allowing the same alias to exist across different genres.
    /// </summary>
    public class GenreAliasConfiguration : IEntityTypeConfiguration<GenreAlias>
    {
        /// <summary>
        /// Configures the database schema and relationships for GenreAlias.
        /// </summary>
        public void Configure(EntityTypeBuilder<GenreAlias> builder)
        {
            /// <summary>
            /// Defines a composite primary key using GenreId and Alias.
            /// 
            /// This enforces that:
            /// - An alias cannot be duplicated within the same genre
            /// - The alias itself participates in the identity of the entity
            /// </summary>
            builder.HasKey(x => new { x.GenreId, x.Alias });

            /// <summary>
            /// Configures the relationship between GenreAlias and Genre.
            /// 
            /// Each alias belongs to exactly one genre,
            /// and a genre can have multiple aliases.
            /// 
            /// No explicit delete behavior is defined here,
            /// so EF Core will apply the default (typically cascade).
            /// </summary>
            builder.HasOne(x => x.Genre).WithMany(x => x.Aliases);
        }
    }
}
