using HotChocolate;
using HotChocolate.Authorization;
using IntegrationGatewayService.GraphQL.Types;
using IntegrationGatewayService.Services;

namespace IntegrationGatewayService.GraphQL.Queries;

/// <summary>
/// GraphQL queries for users
/// </summary>
[ExtendObjectType("Query")]
public class UserQueries
{
    /// <summary>
    /// Get the current authenticated user
    /// </summary>
    [Authorize]
    public async Task<UserType?> GetCurrentUserAsync(
        [Service] IAuthServiceClient authServiceClient,
        CancellationToken cancellationToken = default)
    {
        // JWT token is automatically forwarded by JwtForwardingHandler
        return await authServiceClient.GetCurrentUserAsync(cancellationToken);
    }
}

