namespace AudioAtlasView.Controllers;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using AudioAtlasDomain.Users;

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

    [HttpGet("login/github")]
    public IActionResult GitHubLogin()
    {
        return Challenge(new AuthenticationProperties
        {
            RedirectUri = "/api/auth/external-callback"
        }, "GitHub");
    }

    [HttpGet("login/microsoft")]
    public IActionResult MicrosoftLogin()
    {
        return Challenge(new AuthenticationProperties
        {
            RedirectUri = "/api/auth/external-callback"
        }, "Microsoft");
    }

    [HttpGet("external-callback")]
    public async Task<IActionResult> ExternalCallback()
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();

        if (info == null)
            return Unauthorized();

        var email = info.Principal.FindFirst(ClaimTypes.Email)?.Value;

        if (email == null)
            return BadRequest("Email not provided");

        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email
            };

            var result = await _userManager.CreateAsync(user);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            await _userManager.AddLoginAsync(user, info);
        }

        var token = GenerateJwtToken(user);

        return Ok(new
        {
            token
        });
    }

    private string GenerateJwtToken(ApplicationUser user)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email!)
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}