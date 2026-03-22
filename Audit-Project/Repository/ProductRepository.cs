using Audit_Project.Domain;
using System.Data;

namespace Audit_Project.Repository;

/// <summary>
/// Implements data access for Product entities.
/// </summary>
public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(IDbConnection connection)
        : base(connection, "Products")
    {
    }
}
