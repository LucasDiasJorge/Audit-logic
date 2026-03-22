using Audit_Project.Domain;
using System.Data;

namespace Audit_Project.Repository;

/// <summary>
/// Implements data access for Order entities.
/// </summary>
public class OrderRepository : GenericRepository<Order>, IOrderRepository
{
    public OrderRepository(IDbConnection connection)
        : base(connection, "Orders")
    {
    }
}
