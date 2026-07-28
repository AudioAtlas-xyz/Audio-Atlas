using AudioAtlasDomain.Geography;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AudioAtlasInfrastructure.Database.Configuration.Geography;

/// <summary>
/// Configures the Country entity.
///
/// The isoCode is the natural key used throughout seeding: the supplemental
/// seeder looks countries up by it, so duplicates there are a data-integrity
/// problem rather than a harmless quirk. A duplicated code once aborted the
/// whole supplemental seed part-way through, leaving genres saved but none of
/// their relations linked, so the code is constrained to be unique here.
/// </summary>
public class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        // Bounded length is a prerequisite for the index: the column defaulted to
        // nvarchar(max) by convention, which SQL Server cannot index at all.
        // ISO 3166-1 alpha-3 codes are three characters; the extra headroom
        // tolerates any non-standard codes already in the table.
        builder.Property(c => c.isoCode)
            .IsRequired()
            .HasMaxLength(10);

        builder.HasIndex(c => c.isoCode)
            .IsUnique();
    }
}
