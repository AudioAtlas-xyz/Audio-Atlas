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

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _config;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IConfiguration config)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _config = config;
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

        var email = info.Principal.FindFirst(ClaimTypes.Email)?.Value;

        var username = info.Principal.Identity?.Name ?? email;

        if (username == null)
            return BadRequest("No username or email available");

        var user = await _userManager.FindByLoginAsync(
        info.LoginProvider,
        info.ProviderKey);

        if (user == null)
        {
            user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = username,
                    Email = email ?? $"{username}@noemail.local"
                };

                var result = await _userManager.CreateAsync(user);

                if (!result.Succeeded)
                    return BadRequest(new { errors = result.Errors });
            }

            await _userManager.AddLoginAsync(user, info);
        }

        var token = GenerateJwtToken(user);

        return Ok(new
        {
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
}