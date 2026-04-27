using System.Security.Claims;
using AudioAtlasApplication.DTOs;
using AudioAtlasApplication.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AudioAtlasView.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class SubmissionsController : ControllerBase
{
    private readonly ISubmissionService _submissionService;

    public SubmissionsController(ISubmissionService submissionService)
    {
        _submissionService = submissionService;
    }

    [HttpPost]
    public async Task<ActionResult<CreateSubmissionResponse>> Post(
        [FromBody] CreateSubmissionRequest request,
        CancellationToken cancellationToken)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // user logged in?
        if (!Guid.TryParse(userIdValue, out var accountId))
        {
            return Unauthorized();
        }

        try
        {
            var submissionId = await _submissionService.createSubmissionAsync(
                accountId,
                request,
                cancellationToken);

            var response = new CreateSubmissionResponse
            {
                Id = submissionId
            };

            return StatusCode(StatusCodes.Status201Created, response);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(exception.Message);
        }
    }
}
