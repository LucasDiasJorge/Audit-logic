using Audit_Project.Domain;
using Audit_Project.Audit.Interfaces;
using System.Data;

namespace Audit_Project.Repository;

/// <summary>
/// Implements data access for Client entities.
/// </summary>
public class ClientRepository : GenericRepository<Client>, IClientRepository
{
    public ClientRepository(IDbConnection connection, IAuditService auditService)
        : base(connection, auditService, "Clients")
    {
    }
}
