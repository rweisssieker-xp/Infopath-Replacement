using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FormBuilderService.Models.DTOs;
using FormBuilderService.Services;

namespace FormBuilderService.Controllers;

/// <summary>
/// Forms controller for form CRUD operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FormsController : ControllerBase
{
    private readonly IFormService _formService;
    private readonly ILogger<FormsController> _logger;

    public FormsController(IFormService formService, ILogger<FormsController> logger)
    {
        _formService = formService;
        _logger = logger;
    }

    /// <summary>
    /// Create a new form
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<FormResponse>> CreateForm([FromBody] CreateFormRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            var tenantId = GetTenantId();

            var form = await _formService.CreateFormAsync(request, userId, tenantId, cancellationToken);
            return CreatedAtAction(nameof(GetForm), new { id = form.Id }, form);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = "validation_error", message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating form");
            return StatusCode(500, new { error = "internal_error", message = "An error occurred while creating the form" });
        }
    }

    /// <summary>
    /// Get form by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<FormResponse>> GetForm(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var tenantId = GetTenantId();
            var form = await _formService.GetFormByIdAsync(id, tenantId, cancellationToken);

            if (form == null)
            {
                return NotFound(new { error = "not_found", message = $"Form with id {id} not found" });
            }

            return Ok(form);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting form {FormId}", id);
            return StatusCode(500, new { error = "internal_error", message = "An error occurred while retrieving the form" });
        }
    }

    /// <summary>
    /// Update an existing form
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<FormResponse>> UpdateForm(Guid id, [FromBody] UpdateFormRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            var tenantId = GetTenantId();
            var isAdmin = IsAdmin();

            var form = await _formService.UpdateFormAsync(id, request, userId, tenantId, isAdmin, cancellationToken);
            return Ok(form);
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
            _logger.LogError(ex, "Error updating form {FormId}", id);
            return StatusCode(500, new { error = "internal_error", message = "An error occurred while updating the form" });
        }
    }

    /// <summary>
    /// Soft delete a form
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteForm(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            var tenantId = GetTenantId();
            var isAdmin = IsAdmin();

            var deleted = await _formService.DeleteFormAsync(id, userId, tenantId, isAdmin, cancellationToken);
            if (!deleted)
            {
                return NotFound(new { error = "not_found", message = $"Form with id {id} not found" });
            }

            return Ok(new { success = true });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting form {FormId}", id);
            return StatusCode(500, new { error = "internal_error", message = "An error occurred while deleting the form" });
        }
    }

    /// <summary>
    /// List forms with pagination and filtering
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<FormResponse>>> GetForms(
        [FromQuery] PaginationRequest pagination,
        [FromQuery] FormFilterRequest? filter,
        CancellationToken cancellationToken)
    {
        try
        {
            var tenantId = GetTenantId();
            var result = await _formService.GetFormsAsync(pagination, filter, tenantId, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing forms");
            return StatusCode(500, new { error = "internal_error", message = "An error occurred while listing forms" });
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
            // In production, this should be extracted from claims or throw an error
            return Guid.Empty;
        }
        return tenantId;
    }

    private bool IsAdmin()
    {
        return User.IsInRole("Admin");
    }
}

