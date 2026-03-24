using System.Data;
using System.Text.Json;
using Dapper;
using Audit_Project.Audit.Interfaces;
using Audit_Project.Audit.Models;
using Audit_Project.Domain;

namespace Audit_Project.Audit;

/// <summary>
/// Writes audit records for entities marked with AuditableEntityAttribute.
/// </summary>
public sealed class AuditService(
    IDbConnection connection,
    IAuditMetadataResolver metadataResolver) : IAuditService
{
    private readonly IDbConnection _connection = connection;
    private readonly IAuditMetadataResolver _metadataResolver = metadataResolver;

    public async Task LogAsync(
        Type entityType,
        object? beforeState,
        object? afterState,
        AuditOperationType operationType,
        string loggedBy,
        IDbTransaction transaction)
    {
        AuditEntityMetadata? metadata = await _metadataResolver.ResolveAsync(entityType, transaction);
        if (metadata is null)
        {
            return;
        }

        object? source = afterState ?? beforeState;
        if (source is null)
        {
            return;
        }

        Dictionary<string, object?> values = BuildBaseLogValues(source, operationType, loggedBy, metadata.EntityIdColumnName);

        if (metadata.LogColumns.Contains("Id"))
        {
            values["Id"] = Guid.NewGuid();
        }

        if (metadata.LogColumns.Contains("LoggedAt"))
        {
            values["LoggedAt"] = DateTime.UtcNow;
        }

        if (metadata.LogColumns.Contains("LoggedBy"))
        {
            values["LoggedBy"] = loggedBy;
        }

        if (source is Order order && metadata.LogColumns.Contains("ProductsSnapshot"))
        {
            values["ProductsSnapshot"] = JsonSerializer.Serialize(order.Products.Select(p => new
            {
                p.Id,
                p.ProductName,
                p.Price
            }));
        }

        List<string> validColumns = values
            .Where(v => metadata.LogColumns.Contains(v.Key))
            .Select(v => v.Key)
            .ToList();

        if (validColumns.Count == 0)
        {
            return;
        }

        string columns = string.Join(", ", validColumns);
        string parameters = string.Join(", ", validColumns.Select(c => $"@{c}"));

        string sql = $"INSERT INTO {metadata.LogTableName} ({columns}) VALUES ({parameters})";

        DynamicParameters dbParams = new();
        foreach (string column in validColumns)
        {
            dbParams.Add(column, values[column]);
        }

        await _connection.ExecuteAsync(sql, dbParams, transaction);
    }

    private static Dictionary<string, object?> BuildBaseLogValues(
        object source,
        AuditOperationType operationType,
        string loggedBy,
        string entityIdColumnName)
    {
        Dictionary<string, object?> values = new(StringComparer.OrdinalIgnoreCase)
        {
            ["OperationType"] = ToDatabaseOperation(operationType),
            ["LoggedBy"] = loggedBy
        };

        foreach (var property in source.GetType().GetProperties())
        {
            object? rawValue = property.GetValue(source);

            if (property.Name == "Id")
            {
                values[entityIdColumnName] = rawValue;
                continue;
            }

            if (property.PropertyType != typeof(string) &&
                typeof(System.Collections.IEnumerable).IsAssignableFrom(property.PropertyType))
            {
                continue;
            }

            if (typeof(BaseModel).IsAssignableFrom(property.PropertyType))
            {
                BaseModel? referenced = rawValue as BaseModel;
                values[$"{property.Name}Id"] = referenced?.Id;
                continue;
            }

            values[property.Name] = rawValue;
        }

        return values;
    }

    private static string ToDatabaseOperation(AuditOperationType operationType)
    {
        return operationType switch
        {
            AuditOperationType.Insert => "INSERT",
            AuditOperationType.Update => "UPDATE",
            AuditOperationType.SoftDelete => "SOFT_DELETE",
            _ => "UNKNOWN"
        };
    }
}
