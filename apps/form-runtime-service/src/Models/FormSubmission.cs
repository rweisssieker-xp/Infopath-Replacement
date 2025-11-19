using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FormRuntimeService.Models;

/// <summary>
/// Form submission entity representing submitted form data
/// </summary>
[Table("form_submissions")]
public class FormSubmission
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [Column("form_id")]
    public Guid FormId { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("form_version")]
    public string FormVersion { get; set; } = string.Empty;

    [Required]
    [Column("data", TypeName = "jsonb")]
    public Dictionary<string, object> Data { get; set; } = new();

    [Required]
    [MaxLength(20)]
    [Column("status")]
    public SubmissionStatus Status { get; set; } = SubmissionStatus.Draft;

    [Required]
    [Column("submitted_by")]
    public Guid SubmittedBy { get; set; }

    [Column("submitted_at")]
    public DateTime? SubmittedAt { get; set; }

    [Required]
    [Column("tenant_id")]
    public Guid TenantId { get; set; }

    [MaxLength(255)]
    [Column("workflow_instance_id")]
    public string? WorkflowInstanceId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Column("deleted_at")]
    public DateTime? DeletedAt { get; set; }

    public bool IsDeleted => DeletedAt.HasValue;
}

/// <summary>
/// Submission status enumeration
/// </summary>
public enum SubmissionStatus
{
    Draft = 1,
    Submitted = 2,
    Approved = 3,
    Rejected = 4,
    Completed = 5
}

