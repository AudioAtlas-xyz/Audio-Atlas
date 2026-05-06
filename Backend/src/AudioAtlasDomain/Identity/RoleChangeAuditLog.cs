namespace AudioAtlasDomain.Users;

// Audit row for every admin-initiated role change. Write-only for now;
// no UI reads this back yet.
public class RoleChangeAuditLog
{
    public Guid Id { get; set; } = Guid.NewGuid();

    // Admin who performed the change.
    public Guid ChangedById { get; set; }
    public ApplicationUser? ChangedBy { get; set; }

    // User whose role was changed.
    public Guid TargetUserId { get; set; }
    public ApplicationUser? TargetUser { get; set; }

    // Resolved display roles ("Admin" / "Curator" / "Banned" / "Contributor")
    // matching what the admin UI shows. Stored as plain strings so a
    // future role addition doesn't break old audit rows.
    public string PreviousRole { get; set; } = string.Empty;
    public string NewRole { get; set; } = string.Empty;

    public DateTime ChangedAtUtc { get; set; } = DateTime.UtcNow;
}
