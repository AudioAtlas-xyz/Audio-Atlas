using AudioAtlasDomain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AudioAtlasInfrastructure.Database.Configuration.Identity;

public class RoleChangeAuditLogConfiguration : IEntityTypeConfiguration<RoleChangeAuditLog>
{
    public void Configure(EntityTypeBuilder<RoleChangeAuditLog> builder)
    {
        builder.ToTable("RoleChangeAuditLogs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PreviousRole)
            .IsRequired()
            .HasMaxLength(32);

        builder.Property(x => x.NewRole)
            .IsRequired()
            .HasMaxLength(32);

        builder.Property(x => x.ChangedAtUtc)
            .IsRequired();

        // Restrict deletes — we want to keep the audit trail even if the
        // admin or target user is later removed. The FK columns just go
        // dangling; that's fine for write-only audit data.
        builder.HasOne(x => x.ChangedBy)
            .WithMany()
            .HasForeignKey(x => x.ChangedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.TargetUser)
            .WithMany()
            .HasForeignKey(x => x.TargetUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.ChangedAtUtc);
        builder.HasIndex(x => x.TargetUserId);
    }
}
