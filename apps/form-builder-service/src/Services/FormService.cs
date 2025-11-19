using System.Security.Claims;
using FormBuilderService.Models;
using FormBuilderService.Models.DTOs;
using FormBuilderService.Repositories;
using Microsoft.AspNetCore.Http;

namespace FormBuilderService.Services;

/// <summary>
/// Form service implementation
/// </summary>
public class FormService : IFormService
{
    private readonly IFormRepository _formRepository;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public FormService(
        IFormRepository formRepository,
        IAuditLogRepository auditLogRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _formRepository = formRepository;
        _auditLogRepository = auditLogRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<FormResponse> CreateFormAsync(CreateFormRequest request, Guid userId, Guid tenantId, CancellationToken cancellationToken = default)
    {
        // Validate schema is valid JSON (basic check)
        if (request.Schema == null || request.Schema.Count == 0)
        {
            throw new ArgumentException("Schema is required and must be valid JSON", nameof(request));
        }

        var form = new Form
        {
            Name = request.Name,
            Description = request.Description,
            Schema = request.Schema,
            Version = "1.0.0",
            Status = FormStatus.Draft,
            CreatedBy = userId,
            TenantId = tenantId,
            Tags = request.Tags ?? new List<string>(),
            Category = request.Category,
            WorkflowConfig = request.WorkflowConfig,
            IntegrationConfig = request.IntegrationConfig
        };

        var createdForm = await _formRepository.CreateAsync(form, cancellationToken);

        // Log audit
        await _auditLogRepository.CreateAsync(new AuditLog
        {
            EntityType = "Form",
            EntityId = createdForm.Id,
            Action = "Create",
            UserId = userId,
            Metadata = new Dictionary<string, object> { { "formName", createdForm.Name } }
        }, cancellationToken);

        return MapToResponse(createdForm);
    }

    public async Task<FormResponse?> GetFormByIdAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default)
    {
        var form = await _formRepository.GetByIdAsync(id, cancellationToken);
        if (form == null || form.TenantId != tenantId)
        {
            return null;
        }

        return MapToResponse(form);
    }

    public async Task<FormResponse> UpdateFormAsync(Guid id, UpdateFormRequest request, Guid userId, Guid tenantId, bool isAdmin, CancellationToken cancellationToken = default)
    {
        var form = await _formRepository.GetByIdAsync(id, cancellationToken);
        if (form == null)
        {
            throw new KeyNotFoundException($"Form with id {id} not found");
        }

        // Check tenant isolation
        if (form.TenantId != tenantId)
        {
            throw new UnauthorizedAccessException("Access denied: tenant mismatch");
        }

        // Check authorization: only owner or admin can update
        if (!isAdmin && form.CreatedBy != userId)
        {
            throw new UnauthorizedAccessException("Access denied: only form owner or admin can update");
        }

        var changes = new Dictionary<string, object>();

        // Update fields if provided
        if (!string.IsNullOrEmpty(request.Name) && request.Name != form.Name)
        {
            changes["name"] = new { Old = form.Name, New = request.Name };
            form.Name = request.Name;
        }

        if (request.Description != null && request.Description != form.Description)
        {
            changes["description"] = new { Old = form.Description, New = request.Description };
            form.Description = request.Description;
        }

        if (request.Schema != null)
        {
            changes["schema"] = new { Old = form.Schema, New = request.Schema };
            form.Schema = request.Schema;
        }

        if (request.Status.HasValue && request.Status.Value != form.Status)
        {
            changes["status"] = new { Old = form.Status.ToString(), New = request.Status.Value.ToString() };
            form.Status = request.Status.Value;
        }

        if (request.Tags != null)
        {
            changes["tags"] = new { Old = form.Tags, New = request.Tags };
            form.Tags = request.Tags;
        }

        if (request.Category != null && request.Category != form.Category)
        {
            changes["category"] = new { Old = form.Category, New = request.Category };
            form.Category = request.Category;
        }

        if (request.WorkflowConfig != null)
        {
            form.WorkflowConfig = request.WorkflowConfig;
        }

        if (request.IntegrationConfig != null)
        {
            form.IntegrationConfig = request.IntegrationConfig;
        }

        var updatedForm = await _formRepository.UpdateAsync(form, cancellationToken);

        // Log audit
        await _auditLogRepository.CreateAsync(new AuditLog
        {
            EntityType = "Form",
            EntityId = updatedForm.Id,
            Action = "Update",
            UserId = userId,
            Changes = changes,
            Metadata = new Dictionary<string, object> { { "formName", updatedForm.Name } }
        }, cancellationToken);

        return MapToResponse(updatedForm);
    }

    public async Task<bool> DeleteFormAsync(Guid id, Guid userId, Guid tenantId, bool isAdmin, CancellationToken cancellationToken = default)
    {
        var form = await _formRepository.GetByIdAsync(id, cancellationToken);
        if (form == null)
        {
            return false;
        }

        // Check tenant isolation
        if (form.TenantId != tenantId)
        {
            throw new UnauthorizedAccessException("Access denied: tenant mismatch");
        }

        // Check authorization: only owner or admin can delete
        if (!isAdmin && form.CreatedBy != userId)
        {
            throw new UnauthorizedAccessException("Access denied: only form owner or admin can delete");
        }

        await _formRepository.SoftDeleteAsync(id, cancellationToken);

        // Log audit
        await _auditLogRepository.CreateAsync(new AuditLog
        {
            EntityType = "Form",
            EntityId = id,
            Action = "Delete",
            UserId = userId,
            Metadata = new Dictionary<string, object> { { "formName", form.Name } }
        }, cancellationToken);

        return true;
    }

    public async Task<PaginatedResponse<FormResponse>> GetFormsAsync(PaginationRequest pagination, FormFilterRequest? filter, Guid tenantId, CancellationToken cancellationToken = default)
    {
        var result = await _formRepository.GetPaginatedAsync(pagination, filter, tenantId, cancellationToken);

        return new PaginatedResponse<FormResponse>
        {
            Items = result.Items.Select(MapToResponse).ToList(),
            TotalCount = result.TotalCount,
            Page = result.Page,
            PageSize = result.PageSize
        };
    }

    private static FormResponse MapToResponse(Form form)
    {
        return new FormResponse
        {
            Id = form.Id,
            Name = form.Name,
            Description = form.Description,
            Schema = form.Schema,
            Version = form.Version,
            Status = form.Status.ToString(),
            CreatedBy = form.CreatedBy,
            TenantId = form.TenantId,
            WorkflowConfig = form.WorkflowConfig,
            IntegrationConfig = form.IntegrationConfig,
            Tags = form.Tags,
            Category = form.Category,
            CreatedAt = form.CreatedAt,
            UpdatedAt = form.UpdatedAt
        };
    }
}

