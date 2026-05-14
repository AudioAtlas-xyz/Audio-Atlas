using AudioAtlasDomain.Users;
using AudioAtlasInfrastructure.Database;
using AudioAtlasView.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace AudioAtlasInfrastructureTests.Identity;

// Integration tests for AdminController.ChangeUserRole. SQLite + real Identity.
public class RoleChangeTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly ServiceProvider _services;

    public RoleChangeTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var services = new ServiceCollection();
        services.AddDbContext<AppDbContext>(options => options.UseSqlite(_connection));
        services
            .AddIdentityCore<ApplicationUser>(opts =>
            {
                opts.User.RequireUniqueEmail = false;
                opts.Password.RequireDigit = false;
                opts.Password.RequiredLength = 1;
                opts.Password.RequireLowercase = false;
                opts.Password.RequireUppercase = false;
                opts.Password.RequireNonAlphanumeric = false;
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<AppDbContext>();

        services.AddLogging();

        _services = services.BuildServiceProvider();

        using var scope = _services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.EnsureCreated();

        // Seed roles
        var roles = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        foreach (var name in new[] { "Admin", "Curator", "Banned" })
        {
            roles.CreateAsync(new IdentityRole<Guid> { Name = name }).GetAwaiter().GetResult();
        }
    }

    // Helpers
    private static AdminController BuildController(
        AppDbContext db,
        UserManager<ApplicationUser> users,
        Guid callerId)
    {
        var controller = new AdminController(db, users);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, callerId.ToString())
                }))
            }
        };
        return controller;
    }

    private async Task<ApplicationUser> CreateUserAsync(
        UserManager<ApplicationUser> users,
        string name,
        string? role = null)
    {
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = name,
            Email = $"{name}@a.com"
        };

        var created = await users.CreateAsync(user);
        Assert.True(created.Succeeded, string.Join(",", created.Errors.Select(e => e.Description)));

        if (role is not null)
        {
            var roleResult = await users.AddToRoleAsync(user, role);
            Assert.True(roleResult.Succeeded);
        }

        return user;
    }

    // Validation
    [Fact]
    public async Task ChangeUserRole_WhenRoleMissing_Returns400()
    {
        using var scope = _services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var caller = await CreateUserAsync(users, "admin", "Admin");
        var target = await CreateUserAsync(users, "target");

        var controller = BuildController(db, users, caller.Id);

        var result = await controller.ChangeUserRole(target.Id, new ChangeRoleRequest());

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task ChangeUserRole_WhenRoleUnknown_Returns400()
    {
        using var scope = _services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var caller = await CreateUserAsync(users, "admin", "Admin");
        var target = await CreateUserAsync(users, "target");

        var controller = BuildController(db, users, caller.Id);

        var result = await controller.ChangeUserRole(
            target.Id,
            new ChangeRoleRequest { Role = "SuperUser" });

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task ChangeUserRole_WhenTargetMissing_Returns404()
    {
        using var scope = _services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var caller = await CreateUserAsync(users, "admin", "Admin");
        var controller = BuildController(db, users, caller.Id);

        var result = await controller.ChangeUserRole(
            Guid.NewGuid(),
            new ChangeRoleRequest { Role = "Curator" });

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task ChangeUserRole_WhenTargetIsSystemUser_Returns404()
    {
        using var scope = _services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var caller = await CreateUserAsync(users, "admin", "Admin");
        var system = await CreateUserAsync(users, "system");
        system.IsSystemUser = true;
        await users.UpdateAsync(system);

        var controller = BuildController(db, users, caller.Id);

        var result = await controller.ChangeUserRole(
            system.Id,
            new ChangeRoleRequest { Role = "Curator" });

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task ChangeUserRole_WhenTargetIsDeletedPlaceholder_Returns404()
    {
        using var scope = _services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var caller = await CreateUserAsync(users, "admin", "Admin");
        var placeholder = await CreateUserAsync(users, "placeholder");
        placeholder.IsDeletedPlaceholder = true;
        await users.UpdateAsync(placeholder);

        var controller = BuildController(db, users, caller.Id);

        var result = await controller.ChangeUserRole(
            placeholder.Id,
            new ChangeRoleRequest { Role = "Curator" });

        Assert.IsType<NotFoundObjectResult>(result);
    }

    // Safety checks
    [Fact]
    public async Task ChangeUserRole_WhenSelfDemote_Returns409()
    {
        using var scope = _services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // Two admins so last-admin guard doesn't fire first
        var caller = await CreateUserAsync(users, "admin1", "Admin");
        await CreateUserAsync(users, "admin2", "Admin");

        var controller = BuildController(db, users, caller.Id);

        var result = await controller.ChangeUserRole(
            caller.Id,
            new ChangeRoleRequest { Role = "Curator" });

        Assert.IsType<ConflictObjectResult>(result);
    }

    [Fact]
    public async Task ChangeUserRole_WhenLastAdminDemoted_Returns409()
    {
        using var scope = _services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // Only the target is admin, so the last-admin guard fires
        var caller = await CreateUserAsync(users, "ops", "Curator");
        var theOneAdmin = await CreateUserAsync(users, "admin", "Admin");

        var controller = BuildController(db, users, caller.Id);

        var result = await controller.ChangeUserRole(
            theOneAdmin.Id,
            new ChangeRoleRequest { Role = "Contributor" });

        Assert.IsType<ConflictObjectResult>(result);
    }

    [Fact]
    public async Task ChangeUserRole_WhenSameRole_ReturnsOkAndDoesNotWriteAudit()
    {
        using var scope = _services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var caller = await CreateUserAsync(users, "admin", "Admin");
        var target = await CreateUserAsync(users, "alice", "Curator");

        var controller = BuildController(db, users, caller.Id);

        var result = await controller.ChangeUserRole(
            target.Id,
            new ChangeRoleRequest { Role = "Curator" });

        Assert.IsType<OkObjectResult>(result);
        Assert.Empty(db.RoleChangeAuditLogs);
    }

    // Happy paths

    [Fact]
    public async Task ChangeUserRole_PromoteContributorToCurator_AppliesRoleAndWritesAudit()
    {
        using var scope = _services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var caller = await CreateUserAsync(users, "admin", "Admin");
        var target = await CreateUserAsync(users, "alice");

        var controller = BuildController(db, users, caller.Id);

        var result = await controller.ChangeUserRole(
            target.Id,
            new ChangeRoleRequest { Role = "Curator" });

        Assert.IsType<OkObjectResult>(result);

        Assert.True(await users.IsInRoleAsync(target, "Curator"));

        var audit = Assert.Single(db.RoleChangeAuditLogs);
        Assert.Equal(caller.Id, audit.ChangedById);
        Assert.Equal(target.Id, audit.TargetUserId);
        Assert.Equal("Contributor", audit.PreviousRole);
        Assert.Equal("Curator", audit.NewRole);
    }

    [Fact]
    public async Task ChangeUserRole_DemoteCuratorToContributor_RemovesRole()
    {
        using var scope = _services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var caller = await CreateUserAsync(users, "admin", "Admin");
        var target = await CreateUserAsync(users, "alice", "Curator");

        var controller = BuildController(db, users, caller.Id);

        var result = await controller.ChangeUserRole(
            target.Id,
            new ChangeRoleRequest { Role = "Contributor" });

        Assert.IsType<OkObjectResult>(result);
        Assert.Empty(await users.GetRolesAsync(target));

        var audit = Assert.Single(db.RoleChangeAuditLogs);
        Assert.Equal("Curator", audit.PreviousRole);
        Assert.Equal("Contributor", audit.NewRole);
    }

    [Fact]
    public async Task ChangeUserRole_BanUser_AppliesBannedRoleAndWritesAudit()
    {
        using var scope = _services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var caller = await CreateUserAsync(users, "admin", "Admin");
        var target = await CreateUserAsync(users, "spammer");

        var controller = BuildController(db, users, caller.Id);

        var result = await controller.ChangeUserRole(
            target.Id,
            new ChangeRoleRequest { Role = "Banned" });

        Assert.IsType<OkObjectResult>(result);
        Assert.True(await users.IsInRoleAsync(target, "Banned"));

        var audit = Assert.Single(db.RoleChangeAuditLogs);
        Assert.Equal("Banned", audit.NewRole);
    }

    [Fact]
    public async Task ChangeUserRole_PromoteToAdmin_StripsPreviousRole()
    {
        using var scope = _services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var caller = await CreateUserAsync(users, "admin", "Admin");
        var target = await CreateUserAsync(users, "alice", "Curator");

        var controller = BuildController(db, users, caller.Id);

        var result = await controller.ChangeUserRole(
            target.Id,
            new ChangeRoleRequest { Role = "Admin" });

        Assert.IsType<OkObjectResult>(result);

        var roles = await users.GetRolesAsync(target);
        Assert.Single(roles);
        Assert.Contains("Admin", roles);
    }

    public void Dispose()
    {
        _services.Dispose();
        _connection.Dispose();
    }
}
