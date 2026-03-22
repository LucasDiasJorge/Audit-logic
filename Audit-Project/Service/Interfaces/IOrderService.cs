using Audit_Project.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Audit_Project.Service.Interfaces;

/// <summary>
/// Interface for order business logic operations.
/// </summary>
public interface IOrderService
{
    Task<Order> GetOrderByIdAsync(Guid id);
    Task<IEnumerable<Order>> GetAllOrdersAsync();
    Task AddOrderAsync(Order order);
    Task UpdateOrderAsync(Order order);
    Task DeleteOrderAsync(Guid id);
}
