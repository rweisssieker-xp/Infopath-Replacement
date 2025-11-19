using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FormRuntimeService.Models.DTOs;
using FormRuntimeService.Services;

namespace FormRuntimeService.Controllers;

/// <summary>
/// Form submissions controller for submission CRUD operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FormSubmissionsController : ControllerBase
{
    private readonly IFormSubmissionService _submissionService;
    private readonly ILogger<FormSubmissionsController> _logger;

    public FormSubmissionsController(IFormSubmissionService submissionService, ILogger<FormSubmissionsController> logger)
    {
        _submissionService = submissionService;
        _logger = logger;
    }

    /// <summary>
    /// Create a new form submission
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<SubmissionResponse>> CreateSubmission([FromBody] CreateSubmissionRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            var tenantId = GetTenantId();

            var submission = await _submissionService.CreateSubmissionAsync(request, userId, tenantId, cancellationToken);
            return CreatedAtAction(nameof(GetSubmission), new { id = submission.Id }, submission);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = "not_found", message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating submission");
            return StatusCode(500, new { error = "internal_error", message = "An error occurred while creating the submission" });
        }
    }

    /// <summary>
    /// Get submission by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<SubmissionResponse>> GetSubmission(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            var tenantId = GetTenantId();
            var isAdmin = IsAdmin();

            var submission = await _submissionService.GetSubmissionByIdAsync(id, userId, tenantId, isAdmin, cancellationToken);

            if (submission == null)
            {
                return NotFound(new { error = "not_found", message = $"Submission with id {id} not found" });
            }

            return Ok(submission);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting submission {SubmissionId}", id);
            return StatusCode(500, new { error = "internal_error", message = "An error occurred while retrieving the submission" });
        }
    }

    /// <summary>
    /// Update an existing submission (autosave)
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<SubmissionResponse>> UpdateSubmission(Guid id, [FromBody] UpdateSubmissionRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            var tenantId = GetTenantId();
            var isAdmin = IsAdmin();

            var submission = await _submissionService.UpdateSubmissionAsync(id, request, userId, tenantId, isAdmin, cancellationToken);
            return Ok(submission);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = "not_found", message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating submission {SubmissionId}", id);
            return StatusCode(500, new { error = "internal_error", message = "An error occurred while updating the submission" });
        }
    }

    /// <summary>
    /// Soft delete a submission
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSubmission(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            var tenantId = GetTenantId();
            var isAdmin = IsAdmin();

            var deleted = await _submissionService.DeleteSubmissionAsync(id, userId, tenantId, isAdmin, cancellationToken);
            if (!deleted)
            {
                return NotFound(new { error = "not_found", message = $"Submission with id {id} not found" });
            }

            return Ok(new { success = true });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting submission {SubmissionId}", id);
            return StatusCode(500, new { error = "internal_error", message = "An error occurred while deleting the submission" });
        }
    }

    /// <summary>
    /// List submissions with pagination and filtering
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<SubmissionResponse>>> GetSubmissions(
        [FromQuery] PaginationRequest pagination,
        [FromQuery] SubmissionFilterRequest? filter,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            var tenantId = GetTenantId();
            var isAdmin = IsAdmin();

            var result = await _submissionService.GetSubmissionsAsync(pagination, filter, userId, tenantId, isAdmin, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing submissions");
            return StatusCode(500, new { error = "internal_error", message = "An error occurred while listing submissions" });
        }
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid user identifier in token");
        }
        return userId;
    }

    private Guid GetTenantId()
    {
        var tenantIdClaim = User.FindFirst("tenant_id")?.Value;
        if (string.IsNullOrEmpty(tenantIdClaim) || !Guid.TryParse(tenantIdClaim, out var tenantId))
        {
            // For now, use a default tenant ID if not present
            return Guid.Empty;
        }
        return tenantId;
    }

    private bool IsAdmin()
    {
        return User.IsInRole("Admin");
    }
}

