namespace FormRuntimeService.Models.DTOs;

/// <summary>
/// Request DTO for updating an existing form submission
/// </summary>
public class UpdateSubmissionRequest
{
    public Dictionary<string, object>? Data { get; set; }
    public SubmissionStatus? Status { get; set; }
}

