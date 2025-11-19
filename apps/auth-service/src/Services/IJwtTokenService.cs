using System.Security.Claims;
using AuthService.Models;

namespace AuthService.Services;

/// <summary>
/// JWT token service interface
/// </summary>
public interface IJwtTokenService
{
    string GenerateAccessToken(User user);
    ClaimsPrincipal? ValidateToken(string token);
    string GetUserIdFromToken(string token);
}

