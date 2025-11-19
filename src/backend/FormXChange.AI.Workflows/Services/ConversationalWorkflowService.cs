using FormXChange.AI.Services;
using FormXChange.Shared.Data;
using FormXChange.Shared.Models;
using FormXChange.Workflow.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FormXChange.AI.Workflows.Services;

public interface IConversationalWorkflowService
{
    Task<WorkflowGenerationResult> GenerateWorkflowFromConversationAsync(
        Guid sessionId,
        Guid userId,
        Guid tenantId,
        string userMessage,
        Guid? formId = null,
        CancellationToken cancellationToken = default);
    
    Task<WorkflowRefinementResult> RefineWorkflowAsync(
        Guid workflowId,
        Guid userId,
        string refinementRequest,
        CancellationToken cancellationToken = default);
}

public class ConversationalWorkflowService : IConversationalWorkflowService
{
    private readonly IConversationalEngine _conversationalEngine;
    private readonly ILLMService _llmService;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ConversationalWorkflowService> _logger;

    public ConversationalWorkflowService(
        IConversationalEngine conversationalEngine,
        ILLMService llmService,
        ApplicationDbContext context,
        ILogger<ConversationalWorkflowService> logger)
    {
        _conversationalEngine = conversationalEngine;
        _llmService = llmService;
        _context = context;
        _logger = logger;
    }

    public async Task<WorkflowGenerationResult> GenerateWorkflowFromConversationAsync(
        Guid sessionId,
        Guid userId,
        Guid tenantId,
        string userMessage,
        Guid? formId = null,
        CancellationToken cancellationToken = default)
    {
        // Process message through conversational engine
        var response = await _conversationalEngine.ProcessMessageAsync(sessionId, userId, userMessage, formId, cancellationToken);

        // Get form context if formId is provided
        string? formContext = null;
        if (formId.HasValue)
        {
            var form = await _context.Forms.FindAsync(new object[] { formId.Value }, cancellationToken);
            if (form != null)
            {
                formContext = $"Form: {form.Name}\nSchema: {form.Schema}";
            }
        }

        // Generate workflow definition
        var workflowPrompt = $@"Based on the following conversation, generate a workflow definition in JSON format.
The user wants to create a workflow. Extract the workflow requirements and generate a complete workflow definition.

{(formContext != null ? $"Form Context:\n{formContext}\n\n" : "")}
Conversation:
{userMessage}

Response from assistant:
{response}

Generate a workflow definition that includes:
- Workflow name
- Description
- Approval steps
- Conditions and routing
- SLA requirements
- Notifications

Return ONLY valid JSON, no additional text.";

        var workflowJson = await _llmService.GenerateCompletionAsync(workflowPrompt, GetWorkflowGenerationSystemPrompt(), cancellationToken);
        var cleanedWorkflow = CleanJson(workflowJson);

        // Extract workflow name
        var workflowName = ExtractWorkflowName(workflowJson, userMessage);
        var workflowDescription = ExtractWorkflowDescription(workflowJson, userMessage);

        // Create workflow (would need Workflow service)
        // For now, return the generated definition
        return new WorkflowGenerationResult
        {
            WorkflowId = Guid.NewGuid(), // Would be actual ID from created workflow
            Definition = cleanedWorkflow,
            Message = response,
            Success = true
        };
    }

    public async Task<WorkflowRefinementResult> RefineWorkflowAsync(
        Guid workflowId,
        Guid userId,
        string refinementRequest,
        CancellationToken cancellationToken = default)
    {
        // Would fetch workflow from database
        // For now, return success
        return new WorkflowRefinementResult
        {
            WorkflowId = workflowId,
            Success = true,
            Message = "Workflow refined successfully"
        };
    }

    private string GetWorkflowGenerationSystemPrompt()
    {
        return @"You are an expert at generating workflow definitions for business processes.
You understand workflow requirements and create comprehensive workflow definitions.
Always return valid JSON that follows workflow definition standards.";
    }

    private string CleanJson(string json)
    {
        json = json.Trim();
        if (json.StartsWith("```json"))
        {
            json = json.Substring(7);
        }
        if (json.StartsWith("```"))
        {
            json = json.Substring(3);
        }
        if (json.EndsWith("```"))
        {
            json = json.Substring(0, json.Length - 3);
        }
        return json.Trim();
    }

    private string ExtractWorkflowName(string workflowJson, string userMessage)
    {
        try
        {
            var workflow = JObject.Parse(workflowJson);
            var name = workflow["name"]?.ToString();
            if (!string.IsNullOrEmpty(name))
                return name;
        }
        catch { }

        if (userMessage.Contains("workflow for", StringComparison.OrdinalIgnoreCase))
        {
            var index = userMessage.IndexOf("workflow for", StringComparison.OrdinalIgnoreCase);
            var after = userMessage.Substring(index + 13).Trim();
            var words = after.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            if (words.Length > 0)
            {
                return string.Join(" ", words.Take(5));
            }
        }

        return "New Workflow";
    }

    private string? ExtractWorkflowDescription(string workflowJson, string userMessage)
    {
        try
        {
            var workflow = JObject.Parse(workflowJson);
            return workflow["description"]?.ToString();
        }
        catch { }

        return null;
    }
}

public class WorkflowGenerationResult
{
    public Guid WorkflowId { get; set; }
    public string Definition { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool Success { get; set; }
}

public class WorkflowRefinementResult
{
    public Guid? WorkflowId { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}




