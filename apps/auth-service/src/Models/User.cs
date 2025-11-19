using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthService.Models;

/// <summary>
/// User entity representing an authenticated user in the system
/// </summary>
[Table("users")]
public class User
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(255)]
    [Column("email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    [Column("display_name")]
    public string DisplayName { get; set; } = string.Empty;

    [Column("roles")]
    public List<Role> Roles { get; set; } = new();

    [Column("attributes", TypeName = "jsonb")]
    public Dictionary<string, object> Attributes { get; set; } = new();

    [Required]
    [Column("tenant_id")]
    public Guid TenantId { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("last_login_at")]
    public DateTime? LastLoginAt { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

