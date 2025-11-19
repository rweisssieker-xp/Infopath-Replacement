using FormRuntimeService.Models;
using FormRuntimeService.Models.DTOs;

namespace FormRuntimeService.Services;

/// <summary>
/// Form submission service interface
/// </summary>
public interface IFormSubmissionService
{
    Task<SubmissionResponse> CreateSubmissionAsync(CreateSubmissionRequest request, Guid userId, Guid tenantId, CancellationToken cancellationToken = default);
    Task<SubmissionResponse?> GetSubmissionByIdAsync(Guid id, Guid userId, Guid tenantId, bool isAdmin, CancellationToken cancellationToken = default);
    Task<SubmissionResponse> UpdateSubmissionAsync(Guid id, UpdateSubmissionRequest request, Guid userId, Guid tenantId, bool isAdmin, CancellationToken cancellationToken = default);
    Task<bool> DeleteSubmissionAsync(Guid id, Guid userId, Guid tenantId, bool isAdmin, CancellationToken cancellationToken = default);
    Task<PaginatedResponse<SubmissionResponse>> GetSubmissionsAsync(PaginationRequest pagination, SubmissionFilterRequest? filter, Guid userId, Guid tenantId, bool isAdmin, CancellationToken cancellationToken = default);
    Task<bool> ValidateFormExistsAsync(Guid formId, CancellationToken cancellationToken = default);
}

