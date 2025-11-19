using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FormBuilderService.Models;

/// <summary>
/// Audit log entity for tracking all form operations
/// </summary>
[Table("audit_logs")]
public class AuditLog
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(50)]
    [Column("entity_type")]
    public string EntityType { get; set; } = string.Empty;

    [Required]
    [Column("entity_id")]
    public Guid EntityId { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("action")]
    public string Action { get; set; } = string.Empty;

    [Required]
    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    [Column("changes", TypeName = "jsonb")]
    public Dictionary<string, object>? Changes { get; set; }

    [Column("metadata", TypeName = "jsonb")]
    public Dictionary<string, object>? Metadata { get; set; }
}

