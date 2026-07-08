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
    [Authorize(Roles = "Admin,Curator")]
    public async Task<ActionResult<ICollection<PendingSubmissionResponse>>> GetPending(CancellationToken cancellationToken)
    {
        var submissions = await _submissionService.getPendingAsync(cancellationToken);
        return Ok(submissions);
    }

    [HttpPost("{id}/approve")]
    [Authorize(Roles = "Admin,Curator")]
    public async Task<IActionResult> Approve(Guid id, CancellationToken cancellationToken)
    {
        var reviewerId = GetReviewerId();
        if (reviewerId is null) return Unauthorized();

        try
        {
            await _submissionService.approveAsync(id, reviewerId.Value, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(exception.Message);
        }
    }

    [HttpPost("{id}/reject")]
    [Authorize(Roles = "Admin,Curator")]
    public async Task<IActionResult> Reject(
        Guid id,
        [FromBody] RejectSubmissionRequest request,
        CancellationToken cancellationToken)
    {
        var reviewerId = GetReviewerId();
        if (reviewerId is null) return Unauthorized();

        try
        {
            await _submissionService.rejectAsync(id, reviewerId.Value, request, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(exception.Message);
        }
    }

    [HttpPost("{id}/hold")]
    [Authorize(Roles = "Admin,Curator")]
    public async Task<IActionResult> Hold(Guid id, CancellationToken cancellationToken)
    {
        var reviewerId = GetReviewerId();
        if (reviewerId is null) return Unauthorized();

        try
        {
            await _submissionService.holdForSensitivityAsync(id, reviewerId.Value, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(exception.Message);
        }
    }

    [HttpGet("rejection-reasons")]
    [Authorize(Roles = "Admin,Curator")]
    public async Task<ActionResult<ICollection<RejectionReasonResponse>>> GetRejectionReasons(CancellationToken cancellationToken)
    {
        var reasons = await _submissionService.getActiveRejectionReasonsAsync(cancellationToken);
        return Ok(reasons);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Curator")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateSubmissionRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _submissionService.updateSubmissionAsync(id, request, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(exception.Message);
        }
    }

    [HttpPost("suggest/{genreId:guid}")]
    public async Task<ActionResult<CreateSubmissionResponse>> SuggestEdit(
        Guid genreId,
        [FromBody] SuggestEditRequest request,
        CancellationToken cancellationToken)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdValue, out var accountId))
            return Unauthorized();

        if (User.IsInRole("Banned"))
        {
            return StatusCode(
                StatusCodes.Status403Forbidden,
                new { message = "Your account is banned from contributing." });
        }

        try
        {
            var submissionId = await _submissionService.suggestEditAsync(
                accountId,
                genreId,
                request,
                cancellationToken);

            return StatusCode(StatusCodes.Status201Created, new CreateSubmissionResponse { Id = submissionId });
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

        if (!Guid.TryParse(userIdValue, out var accountId))
            return Unauthorized();

        // Banned users can browse but can't contribute. Read from the
        // JWT — accepts the same up-to-1-hour staleness as role demotion.
        if (User.IsInRole("Banned"))
        {
            return StatusCode(
                StatusCodes.Status403Forbidden,
                new { message = "Your account is banned from contributing." });
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

    private Guid? GetReviewerId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out var id) ? id : null;
    }
}
