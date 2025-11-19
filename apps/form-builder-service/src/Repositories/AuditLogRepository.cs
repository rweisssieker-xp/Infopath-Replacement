using Microsoft.EntityFrameworkCore;
using FormBuilderService.Data;
using FormBuilderService.Models;

namespace FormBuilderService.Repositories;

/// <summary>
/// Audit log repository implementation
/// </summary>
public class AuditLogRepository : IAuditLogRepository
{
    private readonly FormBuilderDbContext _context;

    public AuditLogRepository(FormBuilderDbContext context)
    {
        _context = context;
    }

    public async Task<AuditLog> CreateAsync(AuditLog auditLog, CancellationToken cancellationToken = default)
    {
        auditLog.Timestamp = DateTime.UtcNow;
        _context.AuditLogs.Add(auditLog);
        await _context.SaveChangesAsync(cancellationToken);
        return auditLog;
    }

    public async Task<List<AuditLog>> GetByEntityAsync(string entityType, Guid entityId, CancellationToken cancellationToken = default)
    {
        return await _context.AuditLogs
            .Where(a => a.EntityType == entityType && a.EntityId == entityId)
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<AuditLog>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.AuditLogs
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync(cancellationToken);
    }
}

