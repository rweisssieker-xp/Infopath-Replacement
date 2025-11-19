using IntegrationGatewayService.GraphQL.Types;

namespace IntegrationGatewayService.Services;

/// <summary>
/// Client interface for communicating with Auth Service
/// </summary>
public interface IAuthServiceClient
{
    Task<UserType?> GetCurrentUserAsync(string jwtToken, CancellationToken cancellationToken = default);
    Task<UserType?> GetUserByIdAsync(Guid userId, string jwtToken, CancellationToken cancellationToken = default);
}

