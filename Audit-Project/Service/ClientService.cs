using Audit_Project.Domain;
using Audit_Project.Repository;
using Audit_Project.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Audit_Project.Service;

/// <summary>
/// Implements business logic for Client entities.
/// </summary>
public class ClientService : IClientService
{
    private readonly IClientRepository _repository;

    public ClientService(IClientRepository repository)
    {
        _repository = repository;
    }

    public Task<Client> GetClientByIdAsync(Guid id) => _repository.GetByIdAsync(id);

    public Task<IEnumerable<Client>> GetAllClientsAsync() => _repository.GetAllAsync();

    public Task AddClientAsync(Client client) => _repository.AddAsync(client);

    public Task UpdateClientAsync(Client client) => _repository.UpdateAsync(client);

    public Task DeleteClientAsync(Guid id) => _repository.DeleteAsync(id);
}
