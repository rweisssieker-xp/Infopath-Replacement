using Microsoft.EntityFrameworkCore;
using FormBuilderService.Models;

namespace FormBuilderService.Data;

/// <summary>
/// Database context for form builder service
/// </summary>
public class FormBuilderDbContext : DbContext
{
    public FormBuilderDbContext(DbContextOptions<FormBuilderDbContext> options) : base(options)
    {
    }

    public DbSet<Form> Forms { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Form entity
        modelBuilder.Entity<Form>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.CreatedBy);
            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.Name);
            // Partial index for non-deleted forms
            entity.HasIndex(e => e.DeletedAt)
                .HasFilter("\"deleted_at\" IS NULL");
            
            entity.Property(e => e.Status)
                .HasConversion<string>();
            
            entity.Property(e => e.Schema)
                .HasColumnType("jsonb");
            
            entity.Property(e => e.WorkflowConfig)
                .HasColumnType("jsonb");
            
            entity.Property(e => e.IntegrationConfig)
                .HasColumnType("jsonb");
            
            entity.Property(e => e.Tags)
                .HasColumnType("text[]");
        });

        // Configure AuditLog entity
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.EntityType, e.EntityId });
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.Timestamp);
            
            entity.Property(e => e.Changes)
                .HasColumnType("jsonb");
            
            entity.Property(e => e.Metadata)
                .HasColumnType("jsonb");
        });
    }
}

