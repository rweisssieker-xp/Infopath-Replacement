using Microsoft.AspNetCore.Mvc;
using FormXChange.AI.Workflows.Services;

namespace FormXChange.AI.Workflows.Controllers;

[ApiController]
[Route("api/ai/workflows")]
public class AIWorkflowsController : ControllerBase
{
    private readonly IConversationalWorkflowService _conversationalWorkflowService;
    private readonly ILogger<AIWorkflowsController> _logger;

    public AIWorkflowsController(
        IConversationalWorkflowService conversationalWorkflowService,
        ILogger<AIWorkflowsController> logger)
    {
        _conversationalWorkflowService = conversationalWorkflowService;
        _logger = logger;
    }

    [HttpPost("conversation")]
    public async Task<IActionResult> GenerateWorkflowFromConversation(
        [FromBody] GenerateWorkflowRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _conversationalWorkflowService.GenerateWorkflowFromConversationAsync(
                request.SessionId,
                request.UserId,
                request.TenantId,
                request.Message,
                request.FormId,
                cancellationToken);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating workflow from conversation");
            return StatusCode(500, new { error = "Failed to generate workflow", message = ex.Message });
        }
    }

    [HttpPost("{workflowId}/refine")]
    public async Task<IActionResult> RefineWorkflow(
        Guid workflowId,
        [FromBody] RefineWorkflowRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _conversationalWorkflowService.RefineWorkflowAsync(
                workflowId,
                request.UserId,
                request.RefinementRequest,
                cancellationToken);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refining workflow");
            return StatusCode(500, new { error = "Failed to refine workflow", message = ex.Message });
        }
    }
}

public class GenerateWorkflowRequest
{
    public Guid SessionId { get; set; }
    public Guid UserId { get; set; }
    public Guid TenantId { get; set; }
    public string Message { get; set; } = string.Empty;
    public Guid? FormId { get; set; }
}

public class RefineWorkflowRequest
{
    public Guid UserId { get; set; }
    public string RefinementRequest { get; set; } = string.Empty;
}




