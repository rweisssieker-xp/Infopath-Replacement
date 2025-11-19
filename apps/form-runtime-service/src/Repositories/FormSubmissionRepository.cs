using Microsoft.EntityFrameworkCore;
using FormRuntimeService.Data;
using FormRuntimeService.Models;
using FormRuntimeService.Models.DTOs;

namespace FormRuntimeService.Repositories;

/// <summary>
/// Form submission repository implementation
/// </summary>
public class FormSubmissionRepository : IFormSubmissionRepository
{
    private readonly FormRuntimeDbContext _context;

    public FormSubmissionRepository(FormRuntimeDbContext context)
    {
        _context = context;
    }

    public async Task<FormSubmission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.FormSubmissions
            .Where(s => s.Id == id && s.DeletedAt == null)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<FormSubmission> CreateAsync(FormSubmission submission, CancellationToken cancellationToken = default)
    {
        submission.CreatedAt = DateTime.UtcNow;
        submission.UpdatedAt = DateTime.UtcNow;
        _context.FormSubmissions.Add(submission);
        await _context.SaveChangesAsync(cancellationToken);
        return submission;
    }

    public async Task<FormSubmission> UpdateAsync(FormSubmission submission, CancellationToken cancellationToken = default)
    {
        submission.UpdatedAt = DateTime.UtcNow;
        if (submission.Status == SubmissionStatus.Submitted && submission.SubmittedAt == null)
        {
            submission.SubmittedAt = DateTime.UtcNow;
        }
        _context.FormSubmissions.Update(submission);
        await _context.SaveChangesAsync(cancellationToken);
        return submission;
    }

    public async Task SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var submission = await GetByIdAsync(id, cancellationToken);
        if (submission != null)
        {
            submission.DeletedAt = DateTime.UtcNow;
            submission.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.FormSubmissions
            .AnyAsync(s => s.Id == id && s.DeletedAt == null, cancellationToken);
    }

    public async Task<PaginatedResponse<FormSubmission>> GetPaginatedAsync(
        PaginationRequest pagination,
        SubmissionFilterRequest? filter,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        var query = _context.FormSubmissions
            .Where(s => s.DeletedAt == null && s.TenantId == tenantId);

        // Apply filters
        if (filter != null)
        {
            if (filter.FormId.HasValue)
            {
                query = query.Where(s => s.FormId == filter.FormId.Value);
            }

            if (filter.Status.HasValue)
            {
                query = query.Where(s => s.Status == filter.Status.Value);
            }

            if (filter.SubmittedBy.HasValue)
            {
                query = query.Where(s => s.SubmittedBy == filter.SubmittedBy.Value);
            }
        }

        // Apply sorting (default: newest first)
        query = query.OrderByDescending(s => s.CreatedAt);

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var items = await query
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResponse<FormSubmission>
        {
            Items = items,
            TotalCount = totalCount,
            Page = pagination.Page,
            PageSize = pagination.PageSize
        };
    }

    public async Task<List<FormSubmission>> GetByFormIdAsync(Guid formId, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.FormSubmissions
            .Where(s => s.FormId == formId && s.TenantId == tenantId && s.DeletedAt == null)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<FormSubmission>> GetByUserAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.FormSubmissions
            .Where(s => s.SubmittedBy == userId && s.TenantId == tenantId && s.DeletedAt == null)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<FormSubmission?> GetDraftByFormAndUserAsync(Guid formId, Guid userId, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.FormSubmissions
            .Where(s => s.FormId == formId 
                && s.SubmittedBy == userId 
                && s.TenantId == tenantId 
                && s.Status == SubmissionStatus.Draft
                && s.DeletedAt == null)
            .OrderByDescending(s => s.UpdatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }
}

