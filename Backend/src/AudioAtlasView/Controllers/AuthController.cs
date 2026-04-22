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

    [AllowAnonymous]
[HttpGet("login/github")]
public IActionResult GitHubLogin()
{
    var redirectUrl = Url.Action(nameof(ExternalCallback));
    var properties = _signInManager.ConfigureExternalAuthenticationProperties(
        "GitHub",
        redirectUrl);

    return Challenge(properties, "GitHub");
}

[AllowAnonymous]
[HttpGet("login/google")]
public IActionResult GoogleLogin()
{
    var redirectUrl = Url.Action(nameof(ExternalCallback));
    var properties = _signInManager.ConfigureExternalAuthenticationProperties(
        "Google",
        redirectUrl);

    return Challenge(properties, "Google");
}

    [AllowAnonymous]
    [HttpGet("external-callback")]
    public async Task<IActionResult> ExternalCallback()
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();

        if (info == null)
            return Unauthorized();

        var user = await _userManager.FindByLoginAsync(
        info.LoginProvider,
        info.ProviderKey);

       if (user != null)
    {
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        var existingToken = GenerateJwtToken(user);

        return Redirect(
            $"{_frontendBaseUrl}/?newUser=false&token={Uri.EscapeDataString(existingToken)}"
        );
    }

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
            $"{_frontendBaseUrl}/?" +
            $"newUser=true" +
            $"&pendingRegistrationId={pendingRegistration.Id}" +
            $"&suggestedUsername={Uri.EscapeDataString(pendingRegistration.SuggestedUsername)}"
        );
    }

    [AllowAnonymous]
    [HttpGet("check-username")]
    public async Task<IActionResult> CheckUsername([FromQuery] string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return BadRequest(new
            {
                available = false,
                message = "Username is required."
            });

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

    [AllowAnonymous]
    [HttpPost("complete-onboarding")]
    public async Task<IActionResult> CompleteOnboarding([FromBody] CompleteOnboardingRequest request)
    {
        if (!request.AcceptedContributionGuidelines || !request.AcceptedPrivacyPolicy)
            return BadRequest("Contribution guidelines and privacy policy must both be accepted.");

        if (!IsValidUsername(request.Username))
            return BadRequest("Username must be 3-20 characters and contain only letters, numbers, or underscores.");

        var pendingRegistration = await _dbContext.PendingExternalRegistrations
            .SingleOrDefaultAsync(x => x.Id == request.PendingRegistrationId);

        if (pendingRegistration == null)
            return NotFound("Pending registration not found.");

        if (pendingRegistration.ExpiresAtUtc <= DateTime.UtcNow)
            return BadRequest("Pending registration has expired. Please restart OAuth login.");

        var existingUser = await _userManager.FindByNameAsync(request.Username);
        if (existingUser != null)
            return Conflict(new { message = "Username is already in use." });

        var user = new ApplicationUser
        {
            UserName = request.Username,
            Email = pendingRegistration.Email,
            AcceptedPrivacyPolicyAtUtc = DateTime.UtcNow,
            AcceptedPrivacyPolicyVersion = CurrentPrivacyPolicyVersion,
            AcceptedContributionGuidelinesAtUtc = DateTime.UtcNow,
            AcceptedContributionGuidelinesVersion = CurrentContributionGuidelinesVersion
        };

        var createResult = await _userManager.CreateAsync(user);
        if (!createResult.Succeeded)
            return BadRequest(new { errors = createResult.Errors });

        var loginInfo = new UserLoginInfo(
            pendingRegistration.LoginProvider,
            pendingRegistration.ProviderKey,
            pendingRegistration.ProviderDisplayName);

        var addLoginResult = await _userManager.AddLoginAsync(user, loginInfo);
        if (!addLoginResult.Succeeded)
            return BadRequest(new { errors = addLoginResult.Errors });

        _dbContext.PendingExternalRegistrations.Remove(pendingRegistration);
        await _dbContext.SaveChangesAsync();

        var token = GenerateJwtToken(user);

        return Ok(new
        {
            requiresOnboarding = false,
            token
        });
    }

    [Authorize]
    [HttpGet("protected")]
    public IActionResult Protected()
    {
        return Ok("You are authenticated");
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;

        return Ok(new
        {
            userId,
            email
        });
    }

    private string GenerateJwtToken(ApplicationUser user)
    {
        var key = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));        
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email!)
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static bool IsValidUsername(string username)
        => UsernameRegex.IsMatch(username);

    private static string BuildSuggestedUsername(string input)
    {
        var sanitizedBuilder = new StringBuilder();
        var previousWasUnderscore = false;

        foreach (var character in input)
        {
            if (char.IsLetterOrDigit(character))
            {
                sanitizedBuilder.Append(character);
                previousWasUnderscore = false;
            }
            else if (!previousWasUnderscore && (character == '_' || character == '-' || character == '.' || char.IsWhiteSpace(character)))
            {
                sanitizedBuilder.Append('_');
                previousWasUnderscore = true;
            }
        }

        var sanitized = sanitizedBuilder.ToString().Trim('_');

        if (string.IsNullOrWhiteSpace(sanitized))
            sanitized = "user";

        if (sanitized.Length < 3)
            sanitized = $"{sanitized}user";

        if (sanitized.Length > 20)
            sanitized = sanitized[..20];

        return sanitized;
    }
}

public sealed class CompleteOnboardingRequest
{
    public Guid PendingRegistrationId { get; set; }

    public string Username { get; set; } = string.Empty;

    public bool AcceptedContributionGuidelines { get; set; }

    public bool AcceptedPrivacyPolicy { get; set; }
}
