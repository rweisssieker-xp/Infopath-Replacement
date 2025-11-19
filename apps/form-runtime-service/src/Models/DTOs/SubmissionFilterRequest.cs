namespace FormRuntimeService.Models.DTOs;

/// <summary>
/// Request DTO for filtering submissions
/// </summary>
public class SubmissionFilterRequest
{
    public Guid? FormId { get; set; }
    public SubmissionStatus? Status { get; set; }
    public Guid? SubmittedBy { get; set; }
}

