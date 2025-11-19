using FormRuntimeService.Models;
using FormRuntimeService.Models.DTOs;

namespace FormRuntimeService.Repositories;

/// <summary>
/// Form submission repository interface
/// </summary>
public interface IFormSubmissionRepository
{
    Task<FormSubmission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<FormSubmission> CreateAsync(FormSubmission submission, CancellationToken cancellationToken = default);
    Task<FormSubmission> UpdateAsync(FormSubmission submission, CancellationToken cancellationToken = default);
    Task SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PaginatedResponse<FormSubmission>> GetPaginatedAsync(
        PaginationRequest pagination,
        SubmissionFilterRequest? filter,
        Guid tenantId,
        CancellationToken cancellationToken = default);
    Task<List<FormSubmission>> GetByFormIdAsync(Guid formId, Guid tenantId, CancellationToken cancellationToken = default);
    Task<List<FormSubmission>> GetByUserAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken = default);
    Task<FormSubmission?> GetDraftByFormAndUserAsync(Guid formId, Guid userId, Guid tenantId, CancellationToken cancellationToken = default);
}

