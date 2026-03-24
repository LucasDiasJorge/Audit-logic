namespace Audit_Project.Repository;

/// <summary>
/// Generic repository interface for basic CRUD operations.
/// </summary>
/// <typeparam name="T">Entity type.</typeparam>
public interface IGenericRepository<T> where T : Audit_Project.Domain.BaseModel
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity, string loggedBy);
    Task UpdateAsync(T entity, string loggedBy);
    Task DeleteAsync(Guid id, string loggedBy);
}
