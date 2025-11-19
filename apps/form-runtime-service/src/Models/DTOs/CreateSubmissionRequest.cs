namespace FormRuntimeService.Models.DTOs;

/// <summary>
/// Request DTO for creating a new form submission
/// </summary>
public class CreateSubmissionRequest
{
    public Guid FormId { get; set; }
    public Dictionary<string, object> Data { get; set; } = new();
    public SubmissionStatus Status { get; set; } = SubmissionStatus.Draft;
}

