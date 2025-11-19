using Microsoft.EntityFrameworkCore;
using FormRuntimeService.Models;

namespace FormRuntimeService.Data;

/// <summary>
/// Database context for form runtime service
/// </summary>
public class FormRuntimeDbContext : DbContext
{
    public FormRuntimeDbContext(DbContextOptions<FormRuntimeDbContext> options) : base(options)
    {
    }

    public DbSet<FormSubmission> FormSubmissions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure FormSubmission entity
        modelBuilder.Entity<FormSubmission>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.FormId);
            entity.HasIndex(e => e.SubmittedBy);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.TenantId);
            // Partial index for non-deleted submissions
            entity.HasIndex(e => e.DeletedAt)
                .HasFilter("\"deleted_at\" IS NULL");
            
            entity.Property(e => e.Status)
                .HasConversion<string>();
            
            entity.Property(e => e.Data)
                .HasColumnType("jsonb");
        });
    }
}

