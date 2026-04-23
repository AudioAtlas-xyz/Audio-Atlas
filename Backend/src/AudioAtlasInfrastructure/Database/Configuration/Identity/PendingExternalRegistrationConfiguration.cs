using AudioAtlasDomain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AudioAtlasInfrastructure.Database.Configuration.Identity;

/// <summary>
/// Configures pending external registrations used during onboarding.
/// </summary>
public class PendingExternalRegistrationConfiguration : IEntityTypeConfiguration<PendingExternalRegistration>
{
    /// <summary>
    /// Configures the table, constraints, and indexes for pending registrations.
    /// </summary>
    public void Configure(EntityTypeBuilder<PendingExternalRegistration> builder)
    {
        builder.ToTable("PendingExternalRegistrations");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.LoginProvider)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ProviderDisplayName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ProviderKey)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.SuggestedUsername)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(x => new { x.LoginProvider, x.ProviderKey })
            .IsUnique();
    }
}
