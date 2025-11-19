using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FormBuilderService.Models;

/// <summary>
/// Form entity representing a form definition with schema, metadata, and configuration
/// </summary>
[Table("forms")]
public class Form
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(255)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("description", TypeName = "text")]
    public string? Description { get; set; }

    [Required]
    [Column("schema", TypeName = "jsonb")]
    public Dictionary<string, object> Schema { get; set; } = new();

    [Required]
    [MaxLength(50)]
    [Column("version")]
    public string Version { get; set; } = "1.0.0";

    [Required]
    [MaxLength(20)]
    [Column("status")]
    public FormStatus Status { get; set; } = FormStatus.Draft;

    [Required]
    [Column("created_by")]
    public Guid CreatedBy { get; set; }

    [Required]
    [Column("tenant_id")]
    public Guid TenantId { get; set; }

    [Column("workflow_config", TypeName = "jsonb")]
    public Dictionary<string, object>? WorkflowConfig { get; set; }

    [Column("integration_config", TypeName = "jsonb")]
    public Dictionary<string, object>? IntegrationConfig { get; set; }

    [Column("tags", TypeName = "text[]")]
    public List<string> Tags { get; set; } = new();

    [MaxLength(100)]
    [Column("category")]
    public string? Category { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Column("deleted_at")]
    public DateTime? DeletedAt { get; set; }

    public bool IsDeleted => DeletedAt.HasValue;
}

/// <summary>
/// Form status enumeration
/// </summary>
public enum FormStatus
{
    Draft = 1,
    Published = 2,
    Archived = 3
}

