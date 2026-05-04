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

    [HttpGet("pending")]
    public async Task<ActionResult<ICollection<PendingSubmissionResponse>>> GetPending(CancellationToken cancellationToken)
    {
        var submissions = await _submissionService.getPendingAsync(cancellationToken);
        return Ok(submissions);
    }

    [HttpPost("{id}/approve")]
    public async Task<IActionResult> Approve(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _submissionService.approveAsync(id, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(exception.Message);
        }
    }

    [HttpPost("{id}/reject")]
    public async Task<IActionResult> Reject(
        Guid id,
        [FromBody] RejectSubmissionRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _submissionService.rejectAsync(id, request, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(exception.Message);
        }
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
