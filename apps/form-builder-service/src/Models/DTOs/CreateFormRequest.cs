namespace FormBuilderService.Models.DTOs;

/// <summary>
/// Request DTO for creating a new form
/// </summary>
public class CreateFormRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Dictionary<string, object> Schema { get; set; } = new();
    public List<string>? Tags { get; set; }
    public string? Category { get; set; }
    public Dictionary<string, object>? WorkflowConfig { get; set; }
    public Dictionary<string, object>? IntegrationConfig { get; set; }
}

