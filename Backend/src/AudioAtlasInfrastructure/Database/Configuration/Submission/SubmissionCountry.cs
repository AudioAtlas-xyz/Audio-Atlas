using AudioAtlasDomain.Submissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AudioAtlasInfrastructure.Database.Configurations
{
    public class SubmissionCountryConfiguration : IEntityTypeConfiguration<SubmissionCountry>
    {
        public void Configure(EntityTypeBuilder<SubmissionCountry> builder)
        {
            builder.HasKey(x => new { x.SubmissionId, x.CountryId });

            builder.HasOne(x => x.Submission)
                .WithMany(x => x.Countries)
                .HasForeignKey(x => x.SubmissionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Country)
                .WithMany()
                .HasForeignKey(x => x.CountryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}