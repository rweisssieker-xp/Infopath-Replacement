using FormBuilderService.Models;
using FormBuilderService.Models.DTOs;

namespace FormBuilderService.Repositories;

/// <summary>
/// Form repository interface
/// </summary>
public interface IFormRepository
{
    Task<Form?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Form?> GetByIdIncludeDeletedAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Form> CreateAsync(Form form, CancellationToken cancellationToken = default);
    Task<Form> UpdateAsync(Form form, CancellationToken cancellationToken = default);
    Task SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PaginatedResponse<Form>> GetPaginatedAsync(
        PaginationRequest pagination,
        FormFilterRequest? filter,
        Guid tenantId,
        CancellationToken cancellationToken = default);
    Task<List<Form>> GetByCreatorAsync(Guid createdBy, Guid tenantId, CancellationToken cancellationToken = default);
    Task<List<Form>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
}

