using AuthService.Models;
using AuthService.Models.DTOs;

namespace AuthService.Services;

/// <summary>
/// User service interface
/// </summary>
public interface IUserService
{
    Task<UserProfileResponse?> GetUserProfileAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<User?> GetOrCreateUserFromAzureAdAsync(string email, string displayName, Dictionary<string, object>? attributes, CancellationToken cancellationToken = default);
    Task<List<string>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default);
}

