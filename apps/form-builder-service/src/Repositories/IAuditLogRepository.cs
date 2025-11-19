using FormBuilderService.Models;

namespace FormBuilderService.Repositories;

/// <summary>
/// Audit log repository interface
/// </summary>
public interface IAuditLogRepository
{
    Task<AuditLog> CreateAsync(AuditLog auditLog, CancellationToken cancellationToken = default);
    Task<List<AuditLog>> GetByEntityAsync(string entityType, Guid entityId, CancellationToken cancellationToken = default);
    Task<List<AuditLog>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default);
}

