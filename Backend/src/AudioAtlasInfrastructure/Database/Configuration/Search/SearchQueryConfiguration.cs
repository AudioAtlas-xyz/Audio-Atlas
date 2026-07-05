using AudioAtlasDomain.Search;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AudioAtlasInfrastructure.Database.Configuration.Search;

public class SearchQueryConfiguration : IEntityTypeConfiguration<SearchQuery>
{
    public void Configure(EntityTypeBuilder<SearchQuery> builder)
    {
        builder.ToTable("SearchQueries");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Term)
            .HasColumnType("nvarchar(200)")
            .IsRequired();

        builder.Property(x => x.NormalizedTerm)
            .HasColumnType("nvarchar(200)")
            .IsRequired();

        builder.Property(x => x.ResultCount)
            .IsRequired();

        builder.Property(x => x.OccurredAt)
            .HasColumnType("datetime2")
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(x => x.ContextRegion)
            .HasColumnType("nvarchar(200)");

        builder.Property(x => x.ContextContinent)
            .HasColumnType("nvarchar(200)");
    }
}
