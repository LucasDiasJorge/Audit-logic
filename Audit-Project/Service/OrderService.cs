using Audit_Project.Domain;
using Audit_Project.Repository;
using Audit_Project.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Audit_Project.Service;

/// <summary>
/// Implements business logic for Order entities.
/// </summary>
public class OrderService : IOrderService
{
    private readonly IOrderRepository _repository;
    public OrderService(IOrderRepository repository)
    {
        _repository = repository;
    }
    public Task<Order> GetOrderByIdAsync(Guid id)
    {
        return _repository.GetByIdAsync(id);
    }

    public Task<IEnumerable<Order>> GetAllOrdersAsync()
    {
        return _repository.GetAllAsync();
    }

    public Task AddOrderAsync(Order order)
    {
        return _repository.AddAsync(order);
    }

    public Task UpdateOrderAsync(Order order)
    {
        return _repository.UpdateAsync(order);
    }

    public Task DeleteOrderAsync(Guid id)
    {
        return _repository.DeleteAsync(id);
    }
}
