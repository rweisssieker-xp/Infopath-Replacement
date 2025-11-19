using HotChocolate;
using HotChocolate.Authorization;
using IntegrationGatewayService.GraphQL.Inputs;
using IntegrationGatewayService.GraphQL.Types;
using IntegrationGatewayService.Services;
using System.Security.Claims;

namespace IntegrationGatewayService.GraphQL.Mutations;

/// <summary>
/// GraphQL mutations for forms
/// </summary>
[ExtendObjectType("Mutation")]
public class FormMutations
{
    /// <summary>
    /// Create a new form
    /// </summary>
    [Authorize]
    public async Task<FormType> CreateFormAsync(
        CreateFormInput input,
        [Service] IFormServiceClient formServiceClient,
        [Service] IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken = default)
    {
        var (userId, tenantId) = ExtractUserContext(httpContextAccessor);
        return await formServiceClient.CreateFormAsync(input, userId, tenantId, cancellationToken);
    }

    /// <summary>
    /// Update an existing form
    /// </summary>
    [Authorize]
    public async Task<FormType> UpdateFormAsync(
        Guid id,
        UpdateFormInput input,
        [Service] IFormServiceClient formServiceClient,
        [Service] IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken = default)
    {
        var (userId, tenantId) = ExtractUserContext(httpContextAccessor);
        return await formServiceClient.UpdateFormAsync(id, input, userId, cancellationToken);
    }

    /// <summary>
    /// Delete a form (soft delete)
    /// </summary>
    [Authorize]
    public async Task<bool> DeleteFormAsync(
        Guid id,
        [Service] IFormServiceClient formServiceClient,
        [Service] IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken = default)
    {
        var (userId, tenantId) = ExtractUserContext(httpContextAccessor);
        return await formServiceClient.DeleteFormAsync(id, userId, cancellationToken);
    }

    private static (Guid userId, Guid tenantId) ExtractUserContext(IHttpContextAccessor httpContextAccessor)
    {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            throw new UnauthorizedAccessException("HTTP context not available");
        }

        var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("User ID not found in token");
        }

        var tenantIdClaim = httpContext.User.FindFirst("tenantid")?.Value;
        var tenantId = !string.IsNullOrEmpty(tenantIdClaim) && Guid.TryParse(tenantIdClaim, out var tid) ? tid : Guid.Empty;

        return (userId, tenantId);
    }
}

