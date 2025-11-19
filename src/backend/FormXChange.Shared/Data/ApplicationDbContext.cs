using Microsoft.EntityFrameworkCore;
using FormXChange.Shared.Models;

namespace FormXChange.Shared.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Form> Forms { get; set; }
    public DbSet<FormVersion> FormVersions { get; set; }
    public DbSet<FormBranch> FormBranches { get; set; }
    public DbSet<FormInstance> FormInstances { get; set; }
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<AIFormGeneration> AIFormGenerations { get; set; }
    public DbSet<AIFormConversation> AIFormConversations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Form configurations
        modelBuilder.Entity<Form>(entity =>
        {
            entity.HasIndex(e => new { e.TenantId, e.Name });
            entity.HasIndex(e => new { e.TenantId, e.Status });
            entity.HasOne(e => e.Tenant)
                .WithMany(t => t.Forms)
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // FormBranch configurations
        modelBuilder.Entity<FormBranch>(entity =>
        {
            entity.HasIndex(e => new { e.FormId, e.Name }).IsUnique();
            entity.HasOne(e => e.Form)
                .WithMany(f => f.Branches)
                .HasForeignKey(e => e.FormId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // FormVersion configurations
        modelBuilder.Entity<FormVersion>(entity =>
        {
            entity.HasIndex(e => new { e.FormId, e.VersionNumber });
            entity.HasOne(e => e.Form)
                .WithMany(f => f.Versions)
                .HasForeignKey(e => e.FormId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // User configurations
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.TenantId);
            entity.HasOne(e => e.Tenant)
                .WithMany(t => t.Users)
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Role configurations
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasIndex(e => new { e.TenantId, e.Name }).IsUnique();
            entity.HasOne(e => e.Tenant)
                .WithMany()
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // AIFormConversation configurations
        modelBuilder.Entity<AIFormConversation>(entity =>
        {
            entity.HasIndex(e => e.SessionId);
            entity.HasIndex(e => new { e.SessionId, e.CreatedAt });
        });
    }
}




