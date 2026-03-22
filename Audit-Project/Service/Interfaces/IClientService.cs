using Audit_Project.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Audit_Project.Service.Interfaces;

/// <summary>
/// Interface for client business logic operations.
/// </summary>
public interface IClientService
{
    Task<Client> GetClientByIdAsync(Guid id);
    Task<IEnumerable<Client>> GetAllClientsAsync();
    Task AddClientAsync(Client client);
    Task UpdateClientAsync(Client client);
    Task DeleteClientAsync(Guid id);
}
