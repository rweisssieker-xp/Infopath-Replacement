using Microsoft.AspNetCore.Mvc;
using FormXChange.Runtime.Services;

namespace FormXChange.Runtime.Controllers;

[ApiController]
[Route("api/runtime/forms")]
public class FormRuntimeController : ControllerBase
{
    private readonly IFormRenderService _renderService;
    private readonly ILogger<FormRuntimeController> _logger;

    public FormRuntimeController(
        IFormRenderService renderService,
        ILogger<FormRuntimeController> logger)
    {
        _renderService = renderService;
        _logger = logger;
    }

    [HttpGet("{formId}/render")]
    public async Task<IActionResult> RenderForm(
        Guid formId,
        [FromQuery] int? version = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _renderService.RenderFormAsync(formId, version, cancellationToken);
            
            if (!result.Success)
                return BadRequest(new { error = result.ErrorMessage });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rendering form");
            return StatusCode(500, new { error = "Failed to render form", message = ex.Message });
        }
    }

    [HttpPost("{formId}/validate")]
    public async Task<IActionResult> ValidateFormData(
        Guid formId,
        [FromBody] ValidateFormRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _renderService.ValidateFormDataAsync(formId, request.FormData, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating form data");
            return StatusCode(500, new { error = "Failed to validate form data", message = ex.Message });
        }
    }

    [HttpPost("render-from-schema")]
    public async Task<IActionResult> RenderFromSchema(
        [FromBody] RenderFromSchemaRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _renderService.RenderFormFromSchemaAsync(request.Schema, cancellationToken);
            
            if (!result.Success)
                return BadRequest(new { error = result.ErrorMessage });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rendering form from schema");
            return StatusCode(500, new { error = "Failed to render form", message = ex.Message });
        }
    }
}

public class ValidateFormRequest
{
    public string FormData { get; set; } = "{}";
}

public class RenderFromSchemaRequest
{
    public string Schema { get; set; } = "{}";
}




