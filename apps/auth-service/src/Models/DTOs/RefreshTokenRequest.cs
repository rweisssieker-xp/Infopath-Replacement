namespace AuthService.Models.DTOs;

/// <summary>
/// Refresh token request DTO
/// </summary>
public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}

