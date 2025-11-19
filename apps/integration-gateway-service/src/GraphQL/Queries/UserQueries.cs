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
        [Service] IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken = default)
    {
        var jwtToken = ExtractJwtToken(httpContextAccessor);
        return await authServiceClient.GetCurrentUserAsync(jwtToken, cancellationToken);
    }

    private static string ExtractJwtToken(IHttpContextAccessor httpContextAccessor)
    {
        var authHeader = httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            throw new UnauthorizedAccessException("JWT token is required");
        }
        return authHeader.Substring("Bearer ".Length).Trim();
    }
}

