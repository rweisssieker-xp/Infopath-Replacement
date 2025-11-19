namespace FormRuntimeService.Models.DTOs;

/// <summary>
/// Response DTO for form submission data
/// </summary>
public class SubmissionResponse
{
    public Guid Id { get; set; }
    public Guid FormId { get; set; }
    public string FormVersion { get; set; } = string.Empty;
    public Dictionary<string, object> Data { get; set; } = new();
    public string Status { get; set; } = string.Empty;
    public Guid SubmittedBy { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public Guid TenantId { get; set; }
    public string? WorkflowInstanceId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

