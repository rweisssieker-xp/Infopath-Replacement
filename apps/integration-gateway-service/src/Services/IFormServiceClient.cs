using IntegrationGatewayService.GraphQL.Inputs;
using IntegrationGatewayService.GraphQL.Types;

namespace IntegrationGatewayService.Services;

/// <summary>
/// Client interface for communicating with Form Builder Service
/// </summary>
public interface IFormServiceClient
{
    Task<List<FormType>> GetFormsAsync(int pageNumber = 1, int pageSize = 10, string? nameFilter = null, FormStatus? statusFilter = null, CancellationToken cancellationToken = default);
    Task<FormType?> GetFormByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<FormType> CreateFormAsync(CreateFormInput input, Guid userId, Guid tenantId, CancellationToken cancellationToken = default);
    Task<FormType> UpdateFormAsync(Guid id, UpdateFormInput input, Guid userId, CancellationToken cancellationToken = default);
    Task<bool> DeleteFormAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
}

