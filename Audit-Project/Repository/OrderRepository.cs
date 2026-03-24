using Audit_Project.Domain;
using Audit_Project.Audit.Interfaces;
using System.Data;

namespace Audit_Project.Repository;

/// <summary>
/// Implements data access for Order entities.
/// </summary>
public class OrderRepository : GenericRepository<Order>, IOrderRepository
{
    public OrderRepository(IDbConnection connection, IAuditService auditService)
        : base(connection, auditService, "Orders")
    {
    }
}
