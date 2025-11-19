using FormXChange.AI.Services;
using FormXChange.Shared.Data;
using FormXChange.Shared.Models;
using FormXChange.Forms.Commands;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FormXChange.AI.Forms.Services;

public interface IConversationalFormService
{
    Task<FormGenerationResult> GenerateFormFromConversationAsync(
        Guid sessionId,
        Guid userId,
        Guid tenantId,
        string userMessage,
        CancellationToken cancellationToken = default);
    
    Task<FormRefinementResult> RefineFormAsync(
        Guid formId,
        Guid userId,
        string refinementRequest,
        CancellationToken cancellationToken = default);
}

public class ConversationalFormService : IConversationalFormService
{
    private readonly IConversationalEngine _conversationalEngine;
    private readonly ILLMService _llmService;
    private readonly IMediator _mediator;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ConversationalFormService> _logger;

    public ConversationalFormService(
        IConversationalEngine conversationalEngine,
        ILLMService llmService,
        IMediator mediator,
        ApplicationDbContext context,
        ILogger<ConversationalFormService> logger)
    {
        _conversationalEngine = conversationalEngine;
        _llmService = llmService;
        _mediator = mediator;
        _context = context;
        _logger = logger;
    }

    public async Task<FormGenerationResult> GenerateFormFromConversationAsync(
        Guid sessionId,
        Guid userId,
        Guid tenantId,
        string userMessage,
        CancellationToken cancellationToken = default)
    {
        // Process message through conversational engine
        var response = await _conversationalEngine.ProcessMessageAsync(sessionId, userId, userMessage, null, cancellationToken);

        // Extract form requirements from conversation
        var formPrompt = $@"Based on the following conversation, generate a JSON Schema for a form.
The user wants to create a form. Extract the form requirements and generate a complete JSON Schema.

Conversation:
{userMessage}

Response from assistant:
{response}

Generate a JSON Schema that includes:
- Form title
- Form description
- All required fields with appropriate types
- Validation rules
- Field labels and placeholders
- Conditional logic if mentioned

Return ONLY valid JSON Schema, no additional text.";

        var schemaJson = await _llmService.GenerateCompletionAsync(formPrompt, GetFormGenerationSystemPrompt(), cancellationToken);

        // Clean and validate JSON Schema
        var cleanedSchema = CleanJsonSchema(schemaJson);
        
        // Extract form name from schema or conversation
        var formName = ExtractFormName(schemaJson, userMessage);
        var formDescription = ExtractFormDescription(schemaJson, userMessage);

        // Create form
        var createCommand = new CreateFormCommand
        {
            Name = formName,
            Description = formDescription,
            Schema = cleanedSchema,
            TenantId = tenantId,
            CreatedBy = userId
        };

        var form = await _mediator.Send(createCommand, cancellationToken);

        // Save AI generation record
        var generation = new AIFormGeneration
        {
            Id = Guid.NewGuid(),
            FormId = form.Id,
            UserId = userId,
            InputText = userMessage,
            GeneratedSchema = cleanedSchema,
            ModelVersion = "gpt-4",
            GeneratedAt = DateTime.UtcNow,
            GenerationType = "Conversational"
        };
        _context.AIFormGenerations.Add(generation);
        await _context.SaveChangesAsync(cancellationToken);

        return new FormGenerationResult
        {
            FormId = form.Id,
            Schema = cleanedSchema,
            Message = response,
            Success = true
        };
    }

    public async Task<FormRefinementResult> RefineFormAsync(
        Guid formId,
        Guid userId,
        string refinementRequest,
        CancellationToken cancellationToken = default)
    {
        var form = await _context.Forms.FindAsync(new object[] { formId }, cancellationToken);
        if (form == null)
        {
            return new FormRefinementResult { Success = false, Message = "Form not found" };
        }

        var refinementPrompt = $@"The user wants to refine an existing form. Current form schema:
{form.Schema}

User request:
{refinementRequest}

Generate an updated JSON Schema that incorporates the requested changes. Return ONLY valid JSON Schema.";

        var updatedSchemaJson = await _llmService.GenerateCompletionAsync(refinementPrompt, GetFormGenerationSystemPrompt(), cancellationToken);
        var cleanedSchema = CleanJsonSchema(updatedSchemaJson);

        // Update form
        var updateCommand = new UpdateFormCommand
        {
            Id = formId,
            Schema = cleanedSchema,
            UpdatedBy = userId
        };

        var updatedForm = await _mediator.Send(updateCommand, cancellationToken);

        return new FormRefinementResult
        {
            FormId = formId,
            UpdatedSchema = cleanedSchema,
            Success = updatedForm != null,
            Message = "Form refined successfully"
        };
    }

    private string GetFormGenerationSystemPrompt()
    {
        return @"You are an expert at generating JSON Schema for forms. 
You understand form requirements and create comprehensive, valid JSON Schema definitions.
Always return valid JSON Schema that follows the JSON Schema specification.
Include appropriate field types, validation rules, and metadata.";
    }

    private string CleanJsonSchema(string schemaJson)
    {
        // Remove markdown code blocks if present
        schemaJson = schemaJson.Trim();
        if (schemaJson.StartsWith("```json"))
        {
            schemaJson = schemaJson.Substring(7);
        }
        if (schemaJson.StartsWith("```"))
        {
            schemaJson = schemaJson.Substring(3);
        }
        if (schemaJson.EndsWith("```"))
        {
            schemaJson = schemaJson.Substring(0, schemaJson.Length - 3);
        }
        schemaJson = schemaJson.Trim();

        // Validate JSON
        try
        {
            var parsed = JObject.Parse(schemaJson);
            return parsed.ToString(Formatting.None);
        }
        catch
        {
            _logger.LogWarning("Failed to parse generated JSON Schema, returning as-is");
            return schemaJson;
        }
    }

    private string ExtractFormName(string schemaJson, string userMessage)
    {
        try
        {
            var schema = JObject.Parse(schemaJson);
            var title = schema["title"]?.ToString();
            if (!string.IsNullOrEmpty(title))
                return title;
        }
        catch { }

        // Fallback: extract from user message
        if (userMessage.Contains("form for", StringComparison.OrdinalIgnoreCase))
        {
            var index = userMessage.IndexOf("form for", StringComparison.OrdinalIgnoreCase);
            var after = userMessage.Substring(index + 9).Trim();
            var words = after.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            if (words.Length > 0)
            {
                return string.Join(" ", words.Take(5));
            }
        }

        return "New Form";
    }

    private string? ExtractFormDescription(string schemaJson, string userMessage)
    {
        try
        {
            var schema = JObject.Parse(schemaJson);
            return schema["description"]?.ToString();
        }
        catch { }

        return null;
    }
}

public class FormGenerationResult
{
    public Guid FormId { get; set; }
    public string Schema { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool Success { get; set; }
}

public class FormRefinementResult
{
    public Guid? FormId { get; set; }
    public string? UpdatedSchema { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}




