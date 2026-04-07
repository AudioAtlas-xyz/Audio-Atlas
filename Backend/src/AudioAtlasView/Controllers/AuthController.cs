namespace AudioAtlasView.Controllers;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    [HttpGet("login/github")]
    public IActionResult GitHubLogin()
    {
        return Challenge(new AuthenticationProperties
        {
            RedirectUri = "/api/auth/me"
        }, "GitHub");
    }

    [HttpGet("login/microsoft")]
    public IActionResult MicrosoftLogin()
    {
        return Challenge(new AuthenticationProperties
        {
            RedirectUri = "/api/auth/me"
        }, "Microsoft");
    }

    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var result = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);

        return Ok(new
        {
            IsAuthenticated = result.Succeeded,
            Name = result.Principal?.Identity?.Name,
            AuthenticationType = result.Principal?.Identity?.AuthenticationType
        });
    }
}