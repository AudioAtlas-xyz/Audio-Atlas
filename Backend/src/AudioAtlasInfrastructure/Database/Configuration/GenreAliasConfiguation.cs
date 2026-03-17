using AudioAtlasDomain.Genres;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AudioAtlasInfrastructure.Database.Configuration;

public class GenreAliasConfiguration : IEntityTypeConfiguration<GenreAlias>
{
    public void Configure(EntityTypeBuilder<GenreAlias> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Alias)
            .IsRequired()
            .HasMaxLength(255);

        builder
            .HasIndex(x => new { x.GenreId, x.Alias })
            .IsUnique();

        builder
            .HasOne(x => x.Genre)
            .WithMany(g => g.Aliases)
            .HasForeignKey(x => x.GenreId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}