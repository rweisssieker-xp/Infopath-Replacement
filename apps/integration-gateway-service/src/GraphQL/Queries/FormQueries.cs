using HotChocolate;
using HotChocolate.Authorization;
using IntegrationGatewayService.GraphQL.Types;
using IntegrationGatewayService.Services;

namespace IntegrationGatewayService.GraphQL.Queries;

/// <summary>
/// GraphQL queries for forms
/// </summary>
[ExtendObjectType("Query")]
public class FormQueries
{
    /// <summary>
    /// Get all forms with optional filtering and pagination
    /// </summary>
    [Authorize]
    public async Task<List<FormType>> GetFormsAsync(
        [Service] IFormServiceClient formServiceClient,
        [Service] IHttpContextAccessor httpContextAccessor,
        int pageNumber = 1,
        int pageSize = 10,
        string? nameFilter = null,
        FormStatus? statusFilter = null,
        CancellationToken cancellationToken = default)
    {
        var jwtToken = ExtractJwtToken(httpContextAccessor);
        return await formServiceClient.GetFormsAsync(pageNumber, pageSize, nameFilter, statusFilter, cancellationToken);
    }

    /// <summary>
    /// Get a form by ID
    /// </summary>
    [Authorize]
    public async Task<FormType?> GetFormAsync(
        Guid id,
        [Service] IFormServiceClient formServiceClient,
        [Service] IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken = default)
    {
        var jwtToken = ExtractJwtToken(httpContextAccessor);
        return await formServiceClient.GetFormByIdAsync(id, cancellationToken);
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

