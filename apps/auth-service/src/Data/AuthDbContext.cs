using Microsoft.EntityFrameworkCore;
using AuthService.Models;

namespace AuthService.Data;

/// <summary>
/// Database context for authentication service
/// </summary>
public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Roles)
                .HasConversion(
                    v => string.Join(",", v.Select(r => r.ToString())),
                    v => v.Split(",", StringSplitOptions.RemoveEmptyEntries)
                        .Select(r => Enum.Parse<Role>(r))
                        .ToList()
                );
            entity.Property(e => e.Attributes)
                .HasColumnType("jsonb");
        });

        // Configure RefreshToken entity
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Token).IsUnique();
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.ExpiresAt);
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}

