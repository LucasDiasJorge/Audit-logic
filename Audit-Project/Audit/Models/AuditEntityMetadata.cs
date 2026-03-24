namespace Audit_Project.Audit.Models;

/// <summary>
/// Resolved metadata for an auditable entity type.
/// </summary>
public sealed class AuditEntityMetadata
{
    public required string MainTableName { get; init; }
    public required string LogTableName { get; init; }
    public required string EntityIdColumnName { get; init; }
    public required HashSet<string> LogColumns { get; init; }
}
