using Audit_Project.Audit.Models;
using System.Data;

namespace Audit_Project.Audit.Interfaces;

/// <summary>
/// Persists audit entries for entities decorated with AuditableEntityAttribute.
/// </summary>
public interface IAuditService
{
    Task LogAsync(
        Type entityType,
        object? beforeState,
        object? afterState,
        AuditOperationType operationType,
        string loggedBy,
        IDbTransaction transaction);
}
