using AudioAtlasDomain.Genres;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AudioAtlasInfrastructure.Database.Configuration.Genre
{
    public class GenreSourceConfiguration : IEntityTypeConfiguration<GenreSource>
    {
        public void Configure(EntityTypeBuilder<GenreSource> builder)
        {
            builder.HasKey(x => new { x.GenreId, x.SourceLink });

            builder.HasOne(x => x.Genre).WithMany(x => x.Sources);
        }




    }
}
