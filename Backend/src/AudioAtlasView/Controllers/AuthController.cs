namespace AudioAtlasView.Controllers;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using AudioAtlasDomain.Users;
using Microsoft.AspNetCore.Authorization;
using AudioAtlasInfrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private const string CurrentPrivacyPolicyVersion = "2026-04";
    private const string CurrentContributionGuidelinesVersion = "2026-04";
    private static readonly Regex UsernameRegex = new("^[a-zA-Z0-9_]{3,20}$", RegexOptions.Compiled);

    private readonly AppDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _config;
    private readonly string _frontendBaseUrl;

    public AuthController(
        AppDbContext dbContext,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IConfiguration config)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _signInManager = signInManager;
        _config = config;

        _frontendBaseUrl = config["Frontend:BaseUrl"] ?? "http://localhost:3000";
    }

    // ================= LOGIN =================

    [AllowAnonymous]
    [HttpGet("login/github")]
    public IActionResult GitHubLogin()
    {
        var redirectUrl = Url.Action(nameof(ExternalCallback), "Auth");
        var properties = _signInManager.ConfigureExternalAuthenticationProperties("GitHub", redirectUrl);

        return Challenge(properties, "GitHub");
    }

    [AllowAnonymous]
    [HttpGet("login/google")]
    public IActionResult GoogleLogin()
    {
        var redirectUrl = Url.Action(nameof(ExternalCallback), "Auth");
        var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);

        return Challenge(properties, "Google");
    }

    // ================= CALLBACK =================

    [AllowAnonymous]
    [HttpGet("external-callback")]
    public async Task<IActionResult> ExternalCallback()
    {
        await _dbContext.PendingExternalRegistrations
            .Where(x => x.ExpiresAtUtc <= DateTime.UtcNow)
            .ExecuteDeleteAsync();

        var info = await _signInManager.GetExternalLoginInfoAsync();

        if (info == null)
            return Unauthorized();

        var user = await _userManager.FindByLoginAsync(
            info.LoginProvider,
            info.ProviderKey);

        if (user != null && user.IsSystemUser)
            return Unauthorized();

        // EXISTING USER
        if (user != null)
        {
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            var token = GenerateJwtToken(user);

            return Redirect($"{_frontendBaseUrl}/auth/callback?token={token}&newUser=false");
        }

        // NEW USER
        var email = info.Principal.FindFirst(ClaimTypes.Email)?.Value;

        if (string.IsNullOrWhiteSpace(email))
            return BadRequest("No verified email available from external provider.");

        var suggestedUsername = BuildSuggestedUsername(info.Principal.Identity?.Name ?? email);

        var pendingRegistration = await _dbContext.PendingExternalRegistrations
            .SingleOrDefaultAsync(x =>
                x.LoginProvider == info.LoginProvider &&
                x.ProviderKey == info.ProviderKey);

        if (pendingRegistration == null)
        {
            pendingRegistration = new PendingExternalRegistration
            {
                Id = Guid.NewGuid(),
                LoginProvider = info.LoginProvider,
                ProviderDisplayName = info.ProviderDisplayName ?? info.LoginProvider,
                ProviderKey = info.ProviderKey,
                Email = email,
                SuggestedUsername = suggestedUsername,
                CreatedAtUtc = DateTime.UtcNow,
                ExpiresAtUtc = DateTime.UtcNow.AddMinutes(15)
            };

            _dbContext.PendingExternalRegistrations.Add(pendingRegistration);
        }
        else
        {
            pendingRegistration.ProviderDisplayName = info.ProviderDisplayName ?? info.LoginProvider;
            pendingRegistration.Email = email;
            pendingRegistration.SuggestedUsername = suggestedUsername;
            pendingRegistration.ExpiresAtUtc = DateTime.UtcNow.AddMinutes(15);
        }

        await _dbContext.SaveChangesAsync();
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        return Redirect(
            $"{_frontendBaseUrl}/auth/callback" +
            $"?newUser=true" +
            $"&pendingRegistrationId={pendingRegistration.Id}" +
            $"&suggestedUsername={Uri.EscapeDataString(pendingRegistration.SuggestedUsername)}"
        );
    }

    // ================= USERNAME =================

    [AllowAnonymous]
    [HttpGet("check-username")]
    public async Task<IActionResult> CheckUsername([FromQuery] string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return BadRequest(new { available = false, message = "Username is required." });

        if (!IsValidUsername(username))
            return Ok(new
            {
                available = false,
                message = "Username must be 3-20 characters and contain only letters, numbers, or underscores."
            });

        var existingUser = await _userManager.FindByNameAsync(username);

        return Ok(new
        {
            available = existingUser == null,
            message = existingUser == null
                ? "Username is available."
                : "Username is already in use."
        });
    }

    public class UpdateUsernameRequest
    {
        public string Username { get; set; } = string.Empty;
    }

    [Authorize]
    [HttpPut("username")]
    public async Task<IActionResult> UpdateUsername([FromBody] UpdateUsernameRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username))
            return BadRequest("Username is required.");

        request.Username = request.Username.Trim();

        if (!IsValidUsername(request.Username))
            return BadRequest("Invalid username format.");

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound();

        var existingUser = await _userManager.FindByNameAsync(request.Username);
        if (existingUser != null && existingUser.Id != user.Id)
            return Conflict("Username already in use.");

        var result = await _userManager.SetUserNameAsync(user, request.Username);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok(new { username = user.UserName });
    }

    // ================= COMPLETE ONBOARDING =================

    [AllowAnonymous]
    [HttpPost("complete-onboarding")]
    public async Task<IActionResult> CompleteOnboarding([FromBody] CompleteOnboardingRequest request)
    {
        if (!request.AcceptedContributionGuidelines || !request.AcceptedPrivacyPolicy)
            return BadRequest("You must accept both policies.");

        if (!IsValidUsername(request.Username))
            return BadRequest("Invalid username format.");

        var pendingRegistration = await _dbContext.PendingExternalRegistrations
            .SingleOrDefaultAsync(x => x.Id == request.PendingRegistrationId);

        if (pendingRegistration == null)
            return NotFound("Pending registration not found.");

        if (pendingRegistration.ExpiresAtUtc <= DateTime.UtcNow)
            return BadRequest("Registration expired.");

        var existingUser = await _userManager.FindByNameAsync(request.Username);
        if (existingUser != null)
            return Conflict(new { message = "Username already in use." });

        var user = new ApplicationUser
        {
            UserName = request.Username,
            Email = pendingRegistration.Email,
            Provider = pendingRegistration.LoginProvider.ToLower(), // ✅ SET PROVIDER
            AcceptedPrivacyPolicyAtUtc = DateTime.UtcNow,
            AcceptedPrivacyPolicyVersion = CurrentPrivacyPolicyVersion,
            AcceptedContributionGuidelinesAtUtc = DateTime.UtcNow,
            AcceptedContributionGuidelinesVersion = CurrentContributionGuidelinesVersion
        };

        var createResult = await _userManager.CreateAsync(user);
        if (!createResult.Succeeded)
            return BadRequest(createResult.Errors);

        var loginInfo = new UserLoginInfo(
            pendingRegistration.LoginProvider,
            pendingRegistration.ProviderKey,
            pendingRegistration.ProviderDisplayName);

        await _userManager.AddLoginAsync(user, loginInfo);

        _dbContext.PendingExternalRegistrations.Remove(pendingRegistration);
        await _dbContext.SaveChangesAsync();

        var token = GenerateJwtToken(user);

        return Ok(new
        {
            requiresOnboarding = false,
            token
        });
    }

    // ================= ME =================

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
            return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            return NotFound();

        return Ok(new
        {
            userId = user.Id,
            email = user.Email,
            username = user.UserName,
            provider = user.Provider
        });
    }

    // ================= JWT =================

    private string GenerateJwtToken(ApplicationUser user)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
        );

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.Name, user.UserName!)
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // ================= HELPERS =================

    private static bool IsValidUsername(string username)
        => UsernameRegex.IsMatch(username);

    private static string BuildSuggestedUsername(string input)
    {
        var builder = new StringBuilder();
        var prevUnderscore = false;

        foreach (var c in input)
        {
            if (char.IsLetterOrDigit(c))
            {
                builder.Append(c);
                prevUnderscore = false;
            }
            else if (!prevUnderscore)
            {
                builder.Append('_');
                prevUnderscore = true;
            }
        }

        var result = builder.ToString().Trim('_');

        if (string.IsNullOrWhiteSpace(result))
            result = "user";

        if (result.Length < 3)
            result += "user";

        if (result.Length > 20)
            result = result[..20];

        return result;
    }
}

public sealed class CompleteOnboardingRequest
{
    public Guid PendingRegistrationId { get; set; }
    public string Username { get; set; } = string.Empty;
    public bool AcceptedContributionGuidelines { get; set; }
    public bool AcceptedPrivacyPolicy { get; set; }
}