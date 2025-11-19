using AuthService.Models;
using AuthService.Models.DTOs;
using AuthService.Repositories;

namespace AuthService.Services;

/// <summary>
/// User service implementation
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserProfileResponse?> GetUserProfileAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            return null;
        }

        return new UserProfileResponse
        {
            Id = user.Id,
            Email = user.Email,
            DisplayName = user.DisplayName,
            Roles = user.Roles.Select(r => r.ToString()).ToList(),
            Attributes = user.Attributes,
            TenantId = user.TenantId
        };
    }

    public async Task<User?> GetOrCreateUserFromAzureAdAsync(
        string email,
        string displayName,
        Dictionary<string, object>? attributes,
        CancellationToken cancellationToken = default)
    {
        var existingUser = await _userRepository.GetByEmailAsync(email, cancellationToken);
        if (existingUser != null)
        {
            // Update last login
            existingUser.LastLoginAt = DateTime.UtcNow;
            existingUser.DisplayName = displayName; // Update display name in case it changed
            if (attributes != null)
            {
                existingUser.Attributes = attributes;
            }
            return await _userRepository.UpdateAsync(existingUser, cancellationToken);
        }

        // Create new user with default role (FormUser)
        // Note: TenantId should be extracted from Azure AD claims or set by admin
        // For now, using a default tenant ID - this should be configured properly
        var newUser = new User
        {
            Email = email,
            DisplayName = displayName,
            Roles = new List<Role> { Role.FormUser }, // Default role
            Attributes = attributes ?? new Dictionary<string, object>(),
            TenantId = Guid.Empty, // TODO: Extract from Azure AD claims
            IsActive = true,
            LastLoginAt = DateTime.UtcNow
        };

        return await _userRepository.CreateAsync(newUser, cancellationToken);
    }

    public async Task<List<string>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            return new List<string>();
        }

        var permissions = new List<string>();

        // Map roles to permissions
        foreach (var role in user.Roles)
        {
            switch (role)
            {
                case Role.Admin:
                    permissions.AddRange(new[] { "form:*", "user:*", "admin:*" });
                    break;
                case Role.FormBuilder:
                    permissions.AddRange(new[] { "form:create", "form:read", "form:update", "form:delete", "form:publish" });
                    break;
                case Role.FormUser:
                    permissions.AddRange(new[] { "form:read", "form:submit", "submission:read", "submission:update" });
                    break;
                case Role.Viewer:
                    permissions.AddRange(new[] { "form:read", "submission:read" });
                    break;
            }
        }

        return permissions.Distinct().ToList();
    }
}

