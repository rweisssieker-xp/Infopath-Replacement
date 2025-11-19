namespace FormBuilderService.Models.DTOs;

/// <summary>
/// Response DTO for form data
/// </summary>
public class FormResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Dictionary<string, object> Schema { get; set; } = new();
    public string Version { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public Guid CreatedBy { get; set; }
    public Guid TenantId { get; set; }
    public Dictionary<string, object>? WorkflowConfig { get; set; }
    public Dictionary<string, object>? IntegrationConfig { get; set; }
    public List<string> Tags { get; set; } = new();
    public string? Category { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

