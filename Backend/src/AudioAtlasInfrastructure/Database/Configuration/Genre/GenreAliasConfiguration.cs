using AudioAtlasDomain.Genres;
using AudioAtlasDomain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace AudioAtlasInfrastructure.Database.Configuration.Genre
{
    public class GenreAliasConfiguration : IEntityTypeConfiguration<GenreAlias>
    {
        public void Configure(EntityTypeBuilder<GenreAlias> builder)
        {
            builder.HasKey(x => new { x.GenreId, x.Alias });

            builder.HasOne(x => x.Genre).WithMany(x => x.Aliases);
        }
    }
}
