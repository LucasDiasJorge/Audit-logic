using System.Data;
using Dapper;
using Audit_Project.Audit.Interfaces;
using Audit_Project.Audit.Models;
using Audit_Project.Domain;

namespace Audit_Project.Repository;

/// <summary>
/// Generic repository implementation for basic CRUD operations using Dapper.
/// </summary>
/// <typeparam name="T">Entity type.</typeparam>
public class GenericRepository<T> : IGenericRepository<T> where T : BaseModel
{
    protected readonly IDbConnection _connection;
    protected readonly string _tableName;
    protected readonly IAuditService _auditService;

    public GenericRepository(IDbConnection connection, IAuditService auditService, string tableName)
    {
        _connection = connection;
        _auditService = auditService;
        _tableName = tableName;
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        string sql = $"SELECT * FROM {_tableName} WHERE Id = @Id";
        return await _connection.QuerySingleOrDefaultAsync<T>(sql, new { Id = id });
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        string sql = $"SELECT * FROM {_tableName}";
        return await _connection.QueryAsync<T>(sql);
    }

    public async Task AddAsync(T entity, string loggedBy)
    {
        entity.CreatedAt = entity.CreatedAt == default ? DateTime.UtcNow : entity.CreatedAt;
        entity.UpdatedAt = DateTime.UtcNow;

        using IDbTransaction transaction = _connection.BeginTransaction();
        try
        {
            Dictionary<string, object?> columns = BuildWriteColumns(entity, includeId: true, includeCollections: false);

            string sql = BuildInsertSql(_tableName, columns.Keys);
            await _connection.ExecuteAsync(sql, columns, transaction);

            await _auditService.LogAsync(typeof(T), null, entity, AuditOperationType.Insert, loggedBy, transaction);

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task UpdateAsync(T entity, string loggedBy)
    {
        using IDbTransaction transaction = _connection.BeginTransaction();
        try
        {
            T? before = await GetByIdTransactionalAsync(entity.Id, transaction);
            if (before is null)
            {
                throw new KeyNotFoundException($"Entity {typeof(T).Name} with id {entity.Id} was not found.");
            }

            entity.UpdatedAt = DateTime.UtcNow;
            Dictionary<string, object?> columns = BuildWriteColumns(entity, includeId: false, includeCollections: false);
            string sql = BuildUpdateSql(_tableName, columns.Keys);
            Dictionary<string, object?> parameters = new(columns, StringComparer.OrdinalIgnoreCase)
            {
                ["Id"] = entity.Id
            };

            await _connection.ExecuteAsync(sql, parameters, transaction);

            T? after = await GetByIdTransactionalAsync(entity.Id, transaction);
            await _auditService.LogAsync(typeof(T), before, after, AuditOperationType.Update, loggedBy, transaction);

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task DeleteAsync(Guid id, string loggedBy)
    {
        using IDbTransaction transaction = _connection.BeginTransaction();
        try
        {
            T? before = await GetByIdTransactionalAsync(id, transaction);
            if (before is null)
            {
                return;
            }

            const string softDeleteTemplate = "UPDATE {0} SET IsDeleted = 1, UpdatedAt = @UpdatedAt WHERE Id = @Id";
            string sql = string.Format(softDeleteTemplate, _tableName);
            await _connection.ExecuteAsync(sql, new { Id = id, UpdatedAt = DateTime.UtcNow }, transaction);

            T? after = await GetByIdTransactionalAsync(id, transaction);
            await _auditService.LogAsync(typeof(T), before, after, AuditOperationType.SoftDelete, loggedBy, transaction);

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    private async Task<T?> GetByIdTransactionalAsync(Guid id, IDbTransaction transaction)
    {
        string sql = $"SELECT * FROM {_tableName} WHERE Id = @Id";
        return await _connection.QuerySingleOrDefaultAsync<T>(sql, new { Id = id }, transaction);
    }

    private static Dictionary<string, object?> BuildWriteColumns(T entity, bool includeId, bool includeCollections)
    {
        Dictionary<string, object?> columns = new(StringComparer.OrdinalIgnoreCase);

        foreach (var property in typeof(T).GetProperties())
        {
            if (!property.CanRead)
            {
                continue;
            }

            if (!includeId && property.Name == nameof(BaseModel.Id))
            {
                continue;
            }

            if (!includeCollections && property.PropertyType != typeof(string) &&
                typeof(System.Collections.IEnumerable).IsAssignableFrom(property.PropertyType))
            {
                continue;
            }

            object? value = property.GetValue(entity);

            if (typeof(BaseModel).IsAssignableFrom(property.PropertyType))
            {
                BaseModel? refEntity = value as BaseModel;
                columns[$"{property.Name}Id"] = refEntity?.Id;
                continue;
            }

            columns[property.Name] = value;
        }

        return columns;
    }

    private static string BuildInsertSql(string tableName, IEnumerable<string> columnNames)
    {
        List<string> columns = columnNames.ToList();
        string columnList = string.Join(", ", columns);
        string paramList = string.Join(", ", columns.Select(c => $"@{c}"));
        return $"INSERT INTO {tableName} ({columnList}) VALUES ({paramList})";
    }

    private static string BuildUpdateSql(string tableName, IEnumerable<string> columnNames)
    {
        string setClause = string.Join(", ", columnNames.Select(c => $"{c} = @{c}"));
        return $"UPDATE {tableName} SET {setClause} WHERE Id = @Id";
    }
}
