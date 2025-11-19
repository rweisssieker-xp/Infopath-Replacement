using Microsoft.EntityFrameworkCore;
using FormBuilderService.Data;
using FormBuilderService.Models;
using FormBuilderService.Models.DTOs;

namespace FormBuilderService.Repositories;

/// <summary>
/// Form repository implementation
/// </summary>
public class FormRepository : IFormRepository
{
    private readonly FormBuilderDbContext _context;

    public FormRepository(FormBuilderDbContext context)
    {
        _context = context;
    }

    public async Task<Form?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Forms
            .Where(f => f.Id == id && f.DeletedAt == null)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Form?> GetByIdIncludeDeletedAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Forms
            .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
    }

    public async Task<Form> CreateAsync(Form form, CancellationToken cancellationToken = default)
    {
        form.CreatedAt = DateTime.UtcNow;
        form.UpdatedAt = DateTime.UtcNow;
        _context.Forms.Add(form);
        await _context.SaveChangesAsync(cancellationToken);
        return form;
    }

    public async Task<Form> UpdateAsync(Form form, CancellationToken cancellationToken = default)
    {
        form.UpdatedAt = DateTime.UtcNow;
        _context.Forms.Update(form);
        await _context.SaveChangesAsync(cancellationToken);
        return form;
    }

    public async Task SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var form = await GetByIdAsync(id, cancellationToken);
        if (form != null)
        {
            form.DeletedAt = DateTime.UtcNow;
            form.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Forms
            .AnyAsync(f => f.Id == id && f.DeletedAt == null, cancellationToken);
    }

    public async Task<PaginatedResponse<Form>> GetPaginatedAsync(
        PaginationRequest pagination,
        FormFilterRequest? filter,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Forms
            .Where(f => f.DeletedAt == null && f.TenantId == tenantId);

        // Apply filters
        if (filter != null)
        {
            if (!string.IsNullOrEmpty(filter.Name))
            {
                query = query.Where(f => f.Name.Contains(filter.Name));
            }

            if (filter.Status.HasValue)
            {
                query = query.Where(f => f.Status == filter.Status.Value);
            }

            if (filter.CreatedBy.HasValue)
            {
                query = query.Where(f => f.CreatedBy == filter.CreatedBy.Value);
            }

            if (filter.Tags != null && filter.Tags.Any())
            {
                query = query.Where(f => f.Tags.Any(t => filter.Tags.Contains(t)));
            }

            if (!string.IsNullOrEmpty(filter.Category))
            {
                query = query.Where(f => f.Category == filter.Category);
            }
        }

        // Apply sorting
        if (!string.IsNullOrEmpty(pagination.SortBy))
        {
            var sortDirection = pagination.SortDirection?.ToLower() == "desc" ? false : true;
            query = pagination.SortBy.ToLower() switch
            {
                "name" => sortDirection ? query.OrderBy(f => f.Name) : query.OrderByDescending(f => f.Name),
                "createdat" => sortDirection ? query.OrderBy(f => f.CreatedAt) : query.OrderByDescending(f => f.CreatedAt),
                "updatedat" => sortDirection ? query.OrderBy(f => f.UpdatedAt) : query.OrderByDescending(f => f.UpdatedAt),
                _ => query.OrderByDescending(f => f.CreatedAt)
            };
        }
        else
        {
            query = query.OrderByDescending(f => f.CreatedAt);
        }

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var items = await query
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResponse<Form>
        {
            Items = items,
            TotalCount = totalCount,
            Page = pagination.Page,
            PageSize = pagination.PageSize
        };
    }

    public async Task<List<Form>> GetByCreatorAsync(Guid createdBy, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.Forms
            .Where(f => f.CreatedBy == createdBy && f.TenantId == tenantId && f.DeletedAt == null)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Form>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.Forms
            .Where(f => f.TenantId == tenantId && f.DeletedAt == null)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}

