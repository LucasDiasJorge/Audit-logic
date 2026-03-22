using Audit_Project.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Audit_Project.Service.Interfaces;

/// <summary>
/// Interface for product business logic operations.
/// </summary>
public interface IProductService
{
    Task<Product> GetProductByIdAsync(Guid id);
    Task<IEnumerable<Product>> GetAllProductsAsync();
    Task AddProductAsync(Product product);
    Task UpdateProductAsync(Product product);
    Task DeleteProductAsync(Guid id);
}
