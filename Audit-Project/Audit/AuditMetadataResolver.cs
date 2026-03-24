using System.Data;
using Dapper;
using Audit_Project.Audit.Interfaces;
using Audit_Project.Audit.Models;

namespace Audit_Project.Audit;

/// <summary>
/// Reads and caches metadata required to write audit rows.
/// </summary>
public sealed class AuditMetadataResolver(IDbConnection connection) : IAuditMetadataResolver
{
    private readonly IDbConnection _connection = connection;
    private readonly Dictionary<Type, AuditEntityMetadata?> _cache = new();

    public async Task<AuditEntityMetadata?> ResolveAsync(Type entityType, IDbTransaction transaction)
    {
        if (_cache.TryGetValue(entityType, out AuditEntityMetadata? cached))
        {
            return cached;
        }

        AuditableEntityAttribute? attribute = entityType
            .GetCustomAttributes(typeof(AuditableEntityAttribute), true)
            .Cast<AuditableEntityAttribute>()
            .FirstOrDefault();

        if (attribute is null)
        {
            _cache[entityType] = null;
            return null;
        }

        string tableName = attribute.TableName;
        string logTableName = attribute.LogTableName;
        string entityIdColumn = $"{entityType.Name}Id";

        const string sql = @"
SELECT COLUMN_NAME
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = DATABASE()
  AND TABLE_NAME = @TableName
ORDER BY ORDINAL_POSITION;";

        IEnumerable<string> columns = await _connection.QueryAsync<string>(
            sql,
            new { TableName = logTableName },
            transaction);

        AuditEntityMetadata metadata = new()
        {
            MainTableName = tableName,
            LogTableName = logTableName,
            EntityIdColumnName = entityIdColumn,
            LogColumns = new HashSet<string>(columns, StringComparer.OrdinalIgnoreCase)
        };

        _cache[entityType] = metadata;
        return metadata;
    }
}
