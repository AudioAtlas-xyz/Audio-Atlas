namespace AudioAtlasView.Controllers;

using System.Security.Claims;
using AudioAtlasDomain.Users;
using AudioAtlasInfrastructure.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// Admin-only endpoints. Class-level [Authorize] gates everything.
[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    // Roles assignable from the admin UI.
    private static readonly string[] AssignableRoles =
        new[] { "Admin", "Curator", "Banned", "Contributor" };

    private readonly AppDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminController(
        AppDbContext dbContext,
        UserManager<ApplicationUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    // GET: api/admin/users
    [HttpGet("users")]
    public async Task<ActionResult<IEnumerable<AdminUserRow>>> GetUsers()
    {
        var users = await _dbContext.Users
            .Where(u => !u.IsSystemUser && !u.IsDeletedPlaceholder)
            .ToListAsync();

        var rows = new List<AdminUserRow>(users.Count);

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);

            rows.Add(new AdminUserRow
            {
                Id = user.Id.ToString(),
                Username = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Role = ResolveDisplayRole(roles),
                MemberSince = user.AcceptedPrivacyPolicyAtUtc?.ToString("o")
            });
        }

        return Ok(rows);
    }

    // PUT: api/admin/users/{id}/role
    [HttpPut("users/{id:guid}/role")]
    public async Task<IActionResult> ChangeUserRole(Guid id, [FromBody] ChangeRoleRequest request)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.Role))
            return BadRequest(new { message = "Role is required." });

        if (!AssignableRoles.Contains(request.Role))
            return BadRequest(new { message = $"Unknown role '{request.Role}'." });

        var target = await _userManager.FindByIdAsync(id.ToString());
        if (target is null || target.IsSystemUser || target.IsDeletedPlaceholder)
            return NotFound(new { message = "User not found." });

        var callerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (callerId is null)
            return Unauthorized();

        var currentRoles = await _userManager.GetRolesAsync(target);
        var previousRole = ResolveDisplayRole(currentRoles);

        // No change
        if (previousRole == request.Role)
            return Ok(new { role = previousRole });

        // Self-demote check
        if (callerId == target.Id.ToString() &&
            previousRole == "Admin" &&
            request.Role != "Admin")
        {
            return Conflict(new { message = "You cannot demote yourself." });
        }

        // Last-admin check
        if (previousRole == "Admin" && request.Role != "Admin")
        {
            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            var remainingAdmins = admins.Count(u => u.Id != target.Id);
            if (remainingAdmins == 0)
                return Conflict(new { message = "At least one Admin must remain." });
        }

        // Apply the change. Strip every Identity role first (Contributor
        // means "no role"), then add the new one if it's a real role.
        var stripResult = await _userManager.RemoveFromRolesAsync(target, currentRoles);
        if (!stripResult.Succeeded)
            return BadRequest(new { message = "Failed to clear existing roles." });

        if (request.Role != "Contributor")
        {
            var addResult = await _userManager.AddToRoleAsync(target, request.Role);
            if (!addResult.Succeeded)
                return BadRequest(new { message = $"Failed to assign role '{request.Role}'." });
        }

        // Invalidate any existing JWTs for the target user so role changes take
        // effect immediately rather than waiting for token expiry.
        await _userManager.UpdateSecurityStampAsync(target);

        // Audit row.
        _dbContext.RoleChangeAuditLogs.Add(new RoleChangeAuditLog
        {
            ChangedById = Guid.Parse(callerId),
            TargetUserId = target.Id,
            PreviousRole = previousRole,
            NewRole = request.Role,
            ChangedAtUtc = DateTime.UtcNow
        });
        await _dbContext.SaveChangesAsync();

        return Ok(new { role = request.Role });
    }

    // A user can hold multiple Identity roles. Pick the most relevant
    // one to show: Admin > Banned > Curator > none.
    private static string ResolveDisplayRole(IList<string> roles)
    {
        if (roles.Contains("Admin")) return "Admin";
        if (roles.Contains("Banned")) return "Banned";
        if (roles.Contains("Curator")) return "Curator";
        return "Contributor";
    }
}

// Shape matches frontend/app/types/admin.ts.
public sealed class AdminUserRow
{
    public string Id { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = "Contributor";
    public string? MemberSince { get; set; }
}

public sealed class ChangeRoleRequest
{
    public string Role { get; set; } = string.Empty;
}
