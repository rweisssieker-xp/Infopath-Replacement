namespace FormBuilderService.Models.DTOs;

/// <summary>
/// Request DTO for filtering forms
/// </summary>
public class FormFilterRequest
{
    public string? Name { get; set; }
    public FormStatus? Status { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? TenantId { get; set; }
    public List<string>? Tags { get; set; }
    public string? Category { get; set; }
}

