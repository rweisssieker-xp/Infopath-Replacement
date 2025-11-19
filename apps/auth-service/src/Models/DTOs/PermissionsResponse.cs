namespace AuthService.Models.DTOs;

/// <summary>
/// Permissions response DTO
/// </summary>
public class PermissionsResponse
{
    public List<string> Permissions { get; set; } = new();
}

