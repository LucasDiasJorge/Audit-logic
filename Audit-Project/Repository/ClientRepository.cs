using Audit_Project.Domain;
using System.Data;

namespace Audit_Project.Repository;

/// <summary>
/// Implements data access for Client entities.
/// </summary>
public class ClientRepository : GenericRepository<Client>, IClientRepository
{
    public ClientRepository(IDbConnection connection)
        : base(connection, "Clients")
    {
    }
}
