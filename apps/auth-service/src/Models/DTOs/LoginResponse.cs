namespace AuthService.Models.DTOs;

/// <summary>
/// Login response DTO containing authentication tokens
/// </summary>
public class LoginResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public int ExpiresIn { get; set; }
    public UserProfileResponse User { get; set; } = new();
}

