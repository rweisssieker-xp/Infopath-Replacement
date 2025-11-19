using System.Security.Claims;
using FormRuntimeService.Models;
using FormRuntimeService.Models.DTOs;
using FormRuntimeService.Repositories;
using Microsoft.AspNetCore.Http;

namespace FormRuntimeService.Services;

/// <summary>
/// Form submission service implementation
/// </summary>
public class FormSubmissionService : IFormSubmissionService
{
    private readonly IFormSubmissionRepository _submissionRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<FormSubmissionService> _logger;

    public FormSubmissionService(
        IFormSubmissionRepository submissionRepository,
        IHttpContextAccessor httpContextAccessor,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<FormSubmissionService> logger)
    {
        _submissionRepository = submissionRepository;
        _httpContextAccessor = httpContextAccessor;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<SubmissionResponse> CreateSubmissionAsync(CreateSubmissionRequest request, Guid userId, Guid tenantId, CancellationToken cancellationToken = default)
    {
        // Validate form exists
        var formExists = await ValidateFormExistsAsync(request.FormId, cancellationToken);
        if (!formExists)
        {
            throw new KeyNotFoundException($"Form with id {request.FormId} not found");
        }

        // Check for existing draft (autosave scenario)
        var existingDraft = await _submissionRepository.GetDraftByFormAndUserAsync(request.FormId, userId, tenantId, cancellationToken);
        if (existingDraft != null)
        {
            // Update existing draft instead of creating new
            existingDraft.Data = request.Data;
            existingDraft.Status = request.Status;
            if (request.Status == SubmissionStatus.Submitted)
            {
                existingDraft.SubmittedAt = DateTime.UtcNow;
            }
            var updated = await _submissionRepository.UpdateAsync(existingDraft, cancellationToken);
            return MapToResponse(updated);
        }

        // Create new submission
        var submission = new FormSubmission
        {
            FormId = request.FormId,
            FormVersion = "1.0.0", // TODO: Fetch actual form version from Form Builder Service
            Data = request.Data,
            Status = request.Status,
            SubmittedBy = userId,
            TenantId = tenantId,
            SubmittedAt = request.Status == SubmissionStatus.Submitted ? DateTime.UtcNow : null
        };

        var createdSubmission = await _submissionRepository.CreateAsync(submission, cancellationToken);
        return MapToResponse(createdSubmission);
    }

    public async Task<SubmissionResponse?> GetSubmissionByIdAsync(Guid id, Guid userId, Guid tenantId, bool isAdmin, CancellationToken cancellationToken = default)
    {
        var submission = await _submissionRepository.GetByIdAsync(id, cancellationToken);
        if (submission == null || submission.TenantId != tenantId)
        {
            return null;
        }

        // Check authorization: only owner or admin can access
        if (!isAdmin && submission.SubmittedBy != userId)
        {
            throw new UnauthorizedAccessException("Access denied: only submission owner or admin can access");
        }

        return MapToResponse(submission);
    }

    public async Task<SubmissionResponse> UpdateSubmissionAsync(Guid id, UpdateSubmissionRequest request, Guid userId, Guid tenantId, bool isAdmin, CancellationToken cancellationToken = default)
    {
        var submission = await _submissionRepository.GetByIdAsync(id, cancellationToken);
        if (submission == null)
        {
            throw new KeyNotFoundException($"Submission with id {id} not found");
        }

        // Check tenant isolation
        if (submission.TenantId != tenantId)
        {
            throw new UnauthorizedAccessException("Access denied: tenant mismatch");
        }

        // Check authorization: only owner or admin can update
        if (!isAdmin && submission.SubmittedBy != userId)
        {
            throw new UnauthorizedAccessException("Access denied: only submission owner or admin can update");
        }

        // Update fields if provided
        if (request.Data != null)
        {
            submission.Data = request.Data;
        }

        if (request.Status.HasValue)
        {
            submission.Status = request.Status.Value;
            if (request.Status.Value == SubmissionStatus.Submitted && submission.SubmittedAt == null)
            {
                submission.SubmittedAt = DateTime.UtcNow;
            }
        }

        var updatedSubmission = await _submissionRepository.UpdateAsync(submission, cancellationToken);
        return MapToResponse(updatedSubmission);
    }

    public async Task<bool> DeleteSubmissionAsync(Guid id, Guid userId, Guid tenantId, bool isAdmin, CancellationToken cancellationToken = default)
    {
        var submission = await _submissionRepository.GetByIdAsync(id, cancellationToken);
        if (submission == null)
        {
            return false;
        }

        // Check tenant isolation
        if (submission.TenantId != tenantId)
        {
            throw new UnauthorizedAccessException("Access denied: tenant mismatch");
        }

        // Check authorization: only owner or admin can delete
        if (!isAdmin && submission.SubmittedBy != userId)
        {
            throw new UnauthorizedAccessException("Access denied: only submission owner or admin can delete");
        }

        await _submissionRepository.SoftDeleteAsync(id, cancellationToken);
        return true;
    }

    public async Task<PaginatedResponse<SubmissionResponse>> GetSubmissionsAsync(PaginationRequest pagination, SubmissionFilterRequest? filter, Guid userId, Guid tenantId, bool isAdmin, CancellationToken cancellationToken = default)
    {
        // If not admin, filter by user
        if (!isAdmin)
        {
            filter ??= new SubmissionFilterRequest();
            filter.SubmittedBy = userId;
        }

        var result = await _submissionRepository.GetPaginatedAsync(pagination, filter, tenantId, cancellationToken);

        return new PaginatedResponse<SubmissionResponse>
        {
            Items = result.Items.Select(MapToResponse).ToList(),
            TotalCount = result.TotalCount,
            Page = result.Page,
            PageSize = result.PageSize
        };
    }

    public async Task<bool> ValidateFormExistsAsync(Guid formId, CancellationToken cancellationToken = default)
    {
        try
        {
            var formBuilderServiceUrl = _configuration["FormBuilderService:BaseUrl"] ?? "http://localhost:5001";
            var httpClient = _httpClientFactory.CreateClient();
            
            // Get auth token from current request
            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (!string.IsNullOrEmpty(token))
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var response = await httpClient.GetAsync($"{formBuilderServiceUrl}/api/forms/{formId}", cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to validate form existence for formId {FormId}", formId);
            // In case of error, assume form exists to avoid blocking submission creation
            // In production, this should be more robust
            return true;
        }
    }

    private static SubmissionResponse MapToResponse(FormSubmission submission)
    {
        return new SubmissionResponse
        {
            Id = submission.Id,
            FormId = submission.FormId,
            FormVersion = submission.FormVersion,
            Data = submission.Data,
            Status = submission.Status.ToString(),
            SubmittedBy = submission.SubmittedBy,
            SubmittedAt = submission.SubmittedAt,
            TenantId = submission.TenantId,
            WorkflowInstanceId = submission.WorkflowInstanceId,
            CreatedAt = submission.CreatedAt,
            UpdatedAt = submission.UpdatedAt
        };
    }
}

