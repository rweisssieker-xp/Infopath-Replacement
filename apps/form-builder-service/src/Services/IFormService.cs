using FormBuilderService.Models;
using FormBuilderService.Models.DTOs;

namespace FormBuilderService.Services;

/// <summary>
/// Form service interface
/// </summary>
public interface IFormService
{
    Task<FormResponse> CreateFormAsync(CreateFormRequest request, Guid userId, Guid tenantId, CancellationToken cancellationToken = default);
    Task<FormResponse?> GetFormByIdAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default);
    Task<FormResponse> UpdateFormAsync(Guid id, UpdateFormRequest request, Guid userId, Guid tenantId, bool isAdmin, CancellationToken cancellationToken = default);
    Task<bool> DeleteFormAsync(Guid id, Guid userId, Guid tenantId, bool isAdmin, CancellationToken cancellationToken = default);
    Task<PaginatedResponse<FormResponse>> GetFormsAsync(PaginationRequest pagination, FormFilterRequest? filter, Guid tenantId, CancellationToken cancellationToken = default);
}

