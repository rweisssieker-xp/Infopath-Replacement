using FormXChange.Shared.Data;
using FormXChange.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;

namespace FormXChange.Runtime.Services;

public interface IFormRenderService
{
    Task<FormRenderResult> RenderFormAsync(Guid formId, int? version = null, CancellationToken cancellationToken = default);
    Task<ValidationResult> ValidateFormDataAsync(Guid formId, string formData, CancellationToken cancellationToken = default);
    Task<FormRenderResult> RenderFormFromSchemaAsync(string schema, CancellationToken cancellationToken = default);
}

public class FormRenderService : IFormRenderService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<FormRenderService> _logger;

    public FormRenderService(
        ApplicationDbContext context,
        ILogger<FormRenderService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<FormRenderResult> RenderFormAsync(Guid formId, int? version = null, CancellationToken cancellationToken = default)
    {
        var form = await _context.Forms
            .Include(f => f.Versions)
            .FirstOrDefaultAsync(f => f.Id == formId && f.IsActive, cancellationToken);

        if (form == null)
        {
            throw new InvalidOperationException($"Form with ID {formId} not found");
        }

        string schema;
        if (version.HasValue)
        {
            var formVersion = form.Versions.FirstOrDefault(v => v.VersionNumber == version.Value);
            schema = formVersion?.Schema ?? form.Schema;
        }
        else
        {
            schema = form.Schema;
        }

        return await RenderFormFromSchemaAsync(schema, cancellationToken);
    }

    public async Task<FormRenderResult> RenderFormFromSchemaAsync(string schema, CancellationToken cancellationToken = default)
    {
        try
        {
            var schemaObj = JObject.Parse(schema);
            var fields = ExtractFields(schemaObj);

            return new FormRenderResult
            {
                Schema = schema,
                Fields = fields,
                Success = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rendering form from schema");
            return new FormRenderResult
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }

    public async Task<ValidationResult> ValidateFormDataAsync(Guid formId, string formData, CancellationToken cancellationToken = default)
    {
        var form = await _context.Forms.FindAsync(new object[] { formId }, cancellationToken);
        if (form == null)
        {
            return new ValidationResult
            {
                IsValid = false,
                Errors = new List<string> { "Form not found" }
            };
        }

        try
        {
            var schema = JObject.Parse(form.Schema);
            var data = JObject.Parse(formData);
            var errors = new List<string>();

            // Basic validation - can be extended with JSON Schema validation library
            if (schema["required"] != null)
            {
                var requiredFields = schema["required"].ToObject<List<string>>();
                foreach (var field in requiredFields)
                {
                    if (data[field] == null || string.IsNullOrWhiteSpace(data[field]?.ToString()))
                    {
                        errors.Add($"Field '{field}' is required");
                    }
                }
            }

            return new ValidationResult
            {
                IsValid = errors.Count == 0,
                Errors = errors
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating form data");
            return new ValidationResult
            {
                IsValid = false,
                Errors = new List<string> { ex.Message }
            };
        }
    }

    private List<FormField> ExtractFields(JObject schema)
    {
        var fields = new List<FormField>();

        if (schema["properties"] != null)
        {
            var properties = schema["properties"] as JObject;
            if (properties != null)
            {
                foreach (var prop in properties.Properties())
                {
                    var field = new FormField
                    {
                        Name = prop.Name,
                        Type = prop.Value["type"]?.ToString() ?? "string",
                        Label = prop.Value["title"]?.ToString() ?? prop.Name,
                        Required = schema["required"]?.ToObject<List<string>>()?.Contains(prop.Name) ?? false,
                        Properties = prop.Value.ToObject<Dictionary<string, object>>()
                    };

                    fields.Add(field);
                }
            }
        }

        return fields;
    }
}

public class FormRenderResult
{
    public bool Success { get; set; }
    public string Schema { get; set; } = string.Empty;
    public List<FormField> Fields { get; set; } = new();
    public string? ErrorMessage { get; set; }
}

public class FormField
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = "string";
    public string Label { get; set; } = string.Empty;
    public bool Required { get; set; }
    public Dictionary<string, object>? Properties { get; set; }
}

public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
}

