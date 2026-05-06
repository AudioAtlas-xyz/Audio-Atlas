namespace AudioAtlasView.Controllers;

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
    private readonly AppDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminController(
        AppDbContext dbContext,
        UserManager<ApplicationUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    // GET: api/admin/users — one row per real user (System and
    // DeletedPlaceholder accounts are filtered out).
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
                // AcceptedPrivacyPolicyAtUtc is set once at signup, so it
                // doubles as a "joined at" timestamp without a new column.
                MemberSince = user.AcceptedPrivacyPolicyAtUtc?.ToString("o")
            });
        }

        return Ok(rows);
    }

    // A user can hold multiple Identity roles. Pick the most relevant
    // one to show: Admin > Banned > Curator > none.
    private static string ResolveDisplayRole(IList<string> roles)
    {
        if (roles.Contains("Admin")) return "Admin";
        if (roles.Contains("Banned")) return "Banned";
        if (roles.Contains("Curator")) return "Curator";
        return "Member";
    }
}

// Shape matches frontend/app/types/admin.ts.
public sealed class AdminUserRow
{
    public string Id { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = "Member";
    public string? MemberSince { get; set; }
}
