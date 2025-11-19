using FormXChange.Shared.Models;
using FormXChange.Shared.Data;
using HotChocolate;
using HotChocolate.Data;
using Microsoft.EntityFrameworkCore;

namespace FormXChange.Api.GraphQL;

[ExtendObjectType(typeof(Query))]
public class FormQueries
{
    [UseDbContext(typeof(ApplicationDbContext))]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Form> GetForms([ScopedService] ApplicationDbContext context)
    {
        return context.Forms
            .Include(f => f.Tenant)
            .Include(f => f.Creator)
            .Where(f => f.IsActive);
    }

    [UseDbContext(typeof(ApplicationDbContext))]
    public async Task<Form?> GetForm(
        Guid id,
        [ScopedService] ApplicationDbContext context)
    {
        return await context.Forms
            .Include(f => f.Tenant)
            .Include(f => f.Creator)
            .Include(f => f.Versions)
            .Include(f => f.Branches)
            .FirstOrDefaultAsync(f => f.Id == id && f.IsActive);
    }

    [UseDbContext(typeof(ApplicationDbContext))]
    public IQueryable<FormVersion> GetFormVersions(
        Guid formId,
        [ScopedService] ApplicationDbContext context)
    {
        return context.FormVersions
            .Where(v => v.FormId == formId)
            .OrderByDescending(v => v.VersionNumber);
    }
}

