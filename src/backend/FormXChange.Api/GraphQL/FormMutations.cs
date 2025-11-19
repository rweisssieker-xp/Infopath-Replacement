using FormXChange.Shared.Models;
using FormXChange.Shared.Data;
using HotChocolate;
using Microsoft.EntityFrameworkCore;

namespace FormXChange.Api.GraphQL;

[ExtendObjectType(typeof(Mutation))]
public class FormMutations
{
    [UseDbContext(typeof(ApplicationDbContext))]
    public async Task<Form> CreateForm(
        string name,
        string? description,
        string schema,
        Guid tenantId,
        Guid createdBy,
        [ScopedService] ApplicationDbContext context)
    {
        var form = new Form
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            Schema = schema,
            TenantId = tenantId,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow,
            Version = 1,
            IsActive = true,
            Status = "Draft"
        };

        context.Forms.Add(form);

        var version = new FormVersion
        {
            Id = Guid.NewGuid(),
            FormId = form.Id,
            VersionNumber = 1,
            Schema = schema,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };

        context.FormVersions.Add(version);
        await context.SaveChangesAsync();

        return form;
    }

    [UseDbContext(typeof(ApplicationDbContext))]
    public async Task<Form?> UpdateForm(
        Guid id,
        string? name,
        string? description,
        string? schema,
        Guid? updatedBy,
        [ScopedService] ApplicationDbContext context)
    {
        var form = await context.Forms.FindAsync(id);
        if (form == null)
            return null;

        if (!string.IsNullOrEmpty(name))
            form.Name = name;

        if (description != null)
            form.Description = description;

        if (!string.IsNullOrEmpty(schema))
        {
            form.Schema = schema;
            form.Version++;

            var version = new FormVersion
            {
                Id = Guid.NewGuid(),
                FormId = form.Id,
                VersionNumber = form.Version,
                Schema = schema,
                CreatedBy = updatedBy ?? form.CreatedBy,
                CreatedAt = DateTime.UtcNow
            };

            context.FormVersions.Add(version);
        }

        if (updatedBy.HasValue)
            form.UpdatedBy = updatedBy.Value;

        form.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();
        return form;
    }
}

