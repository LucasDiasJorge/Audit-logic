namespace Audit_Project.Audit.Models;

/// <summary>
/// Supported audit operation types persisted in *_log tables.
/// </summary>
public enum AuditOperationType
{
    Insert,
    Update,
    SoftDelete
}
