using Microsoft.AspNetCore.Mvc;
using FormXChange.AI.Forms.Services;

namespace FormXChange.AI.Forms.Controllers;

[ApiController]
[Route("api/ai/forms")]
public class AIFormsController : ControllerBase
{
    private readonly IConversationalFormService _conversationalFormService;
    private readonly ILogger<AIFormsController> _logger;

    public AIFormsController(
        IConversationalFormService conversationalFormService,
        ILogger<AIFormsController> logger)
    {
        _conversationalFormService = conversationalFormService;
        _logger = logger;
    }

    [HttpPost("conversation")]
    public async Task<IActionResult> GenerateFormFromConversation(
        [FromBody] GenerateFormRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _conversationalFormService.GenerateFormFromConversationAsync(
                request.SessionId,
                request.UserId,
                request.TenantId,
                request.Message,
                cancellationToken);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating form from conversation");
            return StatusCode(500, new { error = "Failed to generate form", message = ex.Message });
        }
    }

    [HttpPost("{formId}/refine")]
    public async Task<IActionResult> RefineForm(
        Guid formId,
        [FromBody] RefineFormRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _conversationalFormService.RefineFormAsync(
                formId,
                request.UserId,
                request.RefinementRequest,
                cancellationToken);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refining form");
            return StatusCode(500, new { error = "Failed to refine form", message = ex.Message });
        }
    }
}

public class GenerateFormRequest
{
    public Guid SessionId { get; set; }
    public Guid UserId { get; set; }
    public Guid TenantId { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class RefineFormRequest
{
    public Guid UserId { get; set; }
    public string RefinementRequest { get; set; } = string.Empty;
}




