using Audit_Project.Domain;
using Audit_Project.Audit.Interfaces;
using System.Data;

namespace Audit_Project.Repository;

/// <summary>
/// Implements data access for Product entities.
/// </summary>
public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(IDbConnection connection, IAuditService auditService)
        : base(connection, auditService, "Products")
    {
    }
}
