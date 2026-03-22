using System.Data;
using Dapper;

namespace Audit_Project.Repository;

/// <summary>
/// Generic repository implementation for basic CRUD operations using Dapper.
/// </summary>
/// <typeparam name="T">Entity type.</typeparam>
public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly IDbConnection _connection;
    protected readonly string _tableName;

    public GenericRepository(IDbConnection connection, string tableName)
    {
        _connection = connection;
        _tableName = tableName;
    }

    public async Task<T> GetByIdAsync(Guid id)
    {
        string sql = $"SELECT * FROM {_tableName} WHERE Id = @Id";
        return await _connection.QuerySingleOrDefaultAsync<T>(sql, new { Id = id });
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        string sql = $"SELECT * FROM {_tableName}";
        return await _connection.QueryAsync<T>(sql);
    }

    public async Task AddAsync(T entity)
    {
        // This is a placeholder. For real use, implement with reflection or a mapping library.
        throw new NotImplementedException("AddAsync must be implemented for each entity or use a mapping library.");
    }

    public async Task UpdateAsync(T entity)
    {
        // This is a placeholder. For real use, implement with reflection or a mapping library.
        throw new NotImplementedException("UpdateAsync must be implemented for each entity or use a mapping library.");
    }

    public async Task DeleteAsync(Guid id)
    {
        string sql = $"DELETE FROM {_tableName} WHERE Id = @Id";
        await _connection.ExecuteAsync(sql, new { Id = id });
    }
}
