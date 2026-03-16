using AudioAtlasDomain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AudioAtlasInfrastructure.Database.Configuration;

public class FavoriteGenreConfiguration : IEntityTypeConfiguration<FavoriteGenre>
{
    public void Configure(EntityTypeBuilder<FavoriteGenre> builder)
    {
        builder.HasKey(x => new { x.UserId, x.GenreId });

        builder.HasOne(x => x.Genre)
            .WithMany()
            .HasForeignKey(x => x.GenreId);
    }
}
