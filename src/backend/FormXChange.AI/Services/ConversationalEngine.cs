using FormXChange.AI.Services;
using FormXChange.Shared.Data;
using FormXChange.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FormXChange.AI.Services;

public interface IConversationalEngine
{
    Task<string> ProcessMessageAsync(Guid sessionId, Guid userId, string message, Guid? formId = null, CancellationToken cancellationToken = default);
    Task<List<AIFormConversation>> GetConversationHistoryAsync(Guid sessionId, CancellationToken cancellationToken = default);
}

public class ConversationalEngine : IConversationalEngine
{
    private readonly ILLMService _llmService;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ConversationalEngine> _logger;

    public ConversationalEngine(
        ILLMService llmService,
        ApplicationDbContext context,
        ILogger<ConversationalEngine> logger)
    {
        _llmService = llmService;
        _context = context;
        _logger = logger;
    }

    public async Task<string> ProcessMessageAsync(Guid sessionId, Guid userId, string message, Guid? formId = null, CancellationToken cancellationToken = default)
    {
        // Save user message
        var userMessage = new AIFormConversation
        {
            Id = Guid.NewGuid(),
            SessionId = sessionId,
            FormId = formId,
            UserId = userId,
            Message = message,
            Role = "user",
            CreatedAt = DateTime.UtcNow
        };
        _context.AIFormConversations.Add(userMessage);

        // Get conversation history for context
        var history = await GetConversationHistoryAsync(sessionId, cancellationToken);
        var context = BuildContextFromHistory(history);

        // Generate response
        var systemPrompt = GetSystemPrompt(formId);
        var response = await _llmService.GenerateCompletionWithContextAsync(message, context, systemPrompt, cancellationToken);

        // Save assistant response
        var assistantMessage = new AIFormConversation
        {
            Id = Guid.NewGuid(),
            SessionId = sessionId,
            FormId = formId,
            UserId = userId,
            Message = response,
            Role = "assistant",
            CreatedAt = DateTime.UtcNow
        };
        _context.AIFormConversations.Add(assistantMessage);

        await _context.SaveChangesAsync(cancellationToken);

        return response;
    }

    public async Task<List<AIFormConversation>> GetConversationHistoryAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        return await _context.AIFormConversations
            .Where(c => c.SessionId == sessionId)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    private string BuildContextFromHistory(List<AIFormConversation> history)
    {
        if (!history.Any())
            return string.Empty;

        return string.Join("\n", history.Select(h => $"{h.Role}: {h.Message}"));
    }

    private string GetSystemPrompt(Guid? formId)
    {
        if (formId.HasValue)
        {
            return @"You are an AI assistant specialized in creating and refining forms. 
You help users create forms by understanding their requirements and generating JSON Schema definitions.
You can also help refine existing forms by suggesting improvements, adding fields, or modifying validation rules.
Always respond in a helpful and professional manner.";
        }

        return @"You are an AI assistant specialized in creating forms and workflows for enterprise applications.
You help users create forms by understanding their requirements and generating JSON Schema definitions.
You can create forms from natural language descriptions, PDFs, images, or existing templates.
Always respond in a helpful and professional manner. When generating forms, provide valid JSON Schema.";
    }
}




