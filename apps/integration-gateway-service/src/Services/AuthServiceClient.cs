using IntegrationGatewayService.GraphQL.Types;
using System.Net.Http.Headers;

namespace IntegrationGatewayService.Services;

/// <summary>
/// HTTP client for communicating with Auth Service
/// </summary>
public class AuthServiceClient : IAuthServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AuthServiceClient> _logger;

    public AuthServiceClient(HttpClient httpClient, ILogger<AuthServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<UserType?> GetCurrentUserAsync(string jwtToken, CancellationToken cancellationToken = default)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
            var response = await _httpClient.GetAsync("/api/auth/me", cancellationToken);
            
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || 
                response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();
            var userResponse = await response.Content.ReadFromJsonAsync<UserProfileResponseDto>(cancellationToken: cancellationToken);
            return userResponse != null ? MapToUserType(userResponse) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching current user from Auth Service");
            throw;
        }
    }

    public async Task<UserType?> GetUserByIdAsync(Guid userId, string jwtToken, CancellationToken cancellationToken = default)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
            var response = await _httpClient.GetAsync($"/api/auth/users/{userId}", cancellationToken);
            
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();
            var userResponse = await response.Content.ReadFromJsonAsync<UserProfileResponseDto>(cancellationToken: cancellationToken);
            return userResponse != null ? MapToUserType(userResponse) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching user {UserId} from Auth Service", userId);
            throw;
        }
    }

    private static UserType MapToUserType(UserProfileResponseDto dto)
    {
        return new UserType
        {
            Id = dto.Id,
            Email = dto.Email,
            DisplayName = dto.DisplayName,
            Roles = dto.Roles ?? new List<string>(),
            TenantId = dto.TenantId
        };
    }

    private class UserProfileResponseDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public List<string>? Roles { get; set; }
        public Guid TenantId { get; set; }
    }
}

