using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FormXChange.AI.Services;

public interface ILLMService
{
    Task<string> GenerateCompletionAsync(string prompt, string? systemPrompt = null, CancellationToken cancellationToken = default);
    Task<string> GenerateCompletionWithContextAsync(string prompt, string context, string? systemPrompt = null, CancellationToken cancellationToken = default);
}

public class LLMService : ILLMService
{
    private readonly OpenAIClient _client;
    private readonly ILogger<LLMService> _logger;
    private readonly string _modelName;

    public LLMService(IConfiguration configuration, ILogger<LLMService> logger)
    {
        _logger = logger;
        
        var apiKey = configuration["AI:OpenAI:ApiKey"];
        var endpoint = configuration["AI:OpenAI:Endpoint"];
        _modelName = configuration["AI:OpenAI:ModelName"] ?? "gpt-4";

        if (!string.IsNullOrEmpty(endpoint))
        {
            _client = new OpenAIClient(new Uri(endpoint), new Azure.AzureKeyCredential(apiKey ?? ""));
        }
        else if (!string.IsNullOrEmpty(apiKey))
        {
            _client = new OpenAIClient(apiKey);
        }
        else
        {
            throw new InvalidOperationException("OpenAI API key or endpoint must be configured");
        }
    }

    public async Task<string> GenerateCompletionAsync(string prompt, string? systemPrompt = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var chatCompletionsOptions = new ChatCompletionsOptions
            {
                DeploymentName = _modelName,
                Messages =
                {
                    new ChatRequestSystemMessage(systemPrompt ?? "You are a helpful assistant for creating forms and workflows."),
                    new ChatRequestUserMessage(prompt)
                },
                Temperature = 0.7f,
                MaxTokens = 2000
            };

            var response = await _client.GetChatCompletionsAsync(chatCompletionsOptions, cancellationToken);
            return response.Value.Choices[0].Message.Content ?? string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating completion");
            throw;
        }
    }

    public async Task<string> GenerateCompletionWithContextAsync(string prompt, string context, string? systemPrompt = null, CancellationToken cancellationToken = default)
    {
        var enhancedPrompt = $@"Context:
{context}

User Request:
{prompt}";

        return await GenerateCompletionAsync(enhancedPrompt, systemPrompt, cancellationToken);
    }
}




