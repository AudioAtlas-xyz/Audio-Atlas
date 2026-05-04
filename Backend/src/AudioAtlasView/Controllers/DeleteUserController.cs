using AudioAtlasApplication.Services;
using AudioAtlasDomain.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AudioAtlasView.Controllers;

[ApiController]
[Route("api/user")]
public class DeleteUserController : ControllerBase
{
    private readonly IUserDeletionService _userDeletionService;

    public DeleteUserController(IUserDeletionService userDeletionService)
    {
        _userDeletionService = userDeletionService;
    }

    [Authorize]
    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteAccount()
    {
        string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
            return Unauthorized();

        if (!Guid.TryParse(userId, out Guid userGuid))
            return BadRequest("Invalid user ID.");

        bool success = await _userDeletionService.DeleteUserAsync(userGuid);

        if (!success)
            return StatusCode(500, "Account deletion failed.");

        return Ok(new { message = "Account successfully deleted." });
    }
}