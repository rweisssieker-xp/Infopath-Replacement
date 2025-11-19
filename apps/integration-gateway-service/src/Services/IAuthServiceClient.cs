using IntegrationGatewayService.GraphQL.Types;

namespace IntegrationGatewayService.Services;

/// <summary>
/// Client interface for communicating with Auth Service
/// </summary>
public interface IAuthServiceClient
{
    Task<UserType?> GetCurrentUserAsync(CancellationToken cancellationToken = default);
    Task<UserType?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);
}

