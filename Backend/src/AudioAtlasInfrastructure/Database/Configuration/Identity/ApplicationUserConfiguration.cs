using AudioAtlasDomain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AudioAtlasInfrastructure.Database.Configuration.Identity;

/// <summary>
/// Configures custom properties for application users.
/// </summary>
public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    /// <summary>
    /// Configures persistence details for user consent fields.
    /// </summary>
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(x => x.AcceptedPrivacyPolicyVersion)
            .HasMaxLength(50);

        builder.Property(x => x.AcceptedContributionGuidelinesVersion)
            .HasMaxLength(50);
    }
}
