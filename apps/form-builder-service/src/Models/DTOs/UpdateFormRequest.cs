namespace FormBuilderService.Models.DTOs;

/// <summary>
/// Request DTO for updating an existing form
/// </summary>
public class UpdateFormRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public Dictionary<string, object>? Schema { get; set; }
    public FormStatus? Status { get; set; }
    public List<string>? Tags { get; set; }
    public string? Category { get; set; }
    public Dictionary<string, object>? WorkflowConfig { get; set; }
    public Dictionary<string, object>? IntegrationConfig { get; set; }
}

