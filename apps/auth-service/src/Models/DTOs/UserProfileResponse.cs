namespace AuthService.Models.DTOs;

/// <summary>
/// User profile response DTO
/// </summary>
public class UserProfileResponse
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public Dictionary<string, object> Attributes { get; set; } = new();
    public Guid TenantId { get; set; }
}

