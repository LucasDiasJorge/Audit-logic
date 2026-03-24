using Audit_Project.Audit.Models;
using System.Data;

namespace Audit_Project.Audit.Interfaces;

/// <summary>
/// Resolves the log metadata for auditable entity types.
/// </summary>
public interface IAuditMetadataResolver
{
    Task<AuditEntityMetadata?> ResolveAsync(Type entityType, IDbTransaction transaction);
}
