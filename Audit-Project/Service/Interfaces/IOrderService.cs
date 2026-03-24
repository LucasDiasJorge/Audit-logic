using Audit_Project.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Audit_Project.Service.Interfaces;

/// <summary>
/// Interface for order business logic operations.
/// </summary>
public interface IOrderService
{
    Task<Order?> GetOrderByIdAsync(Guid id);
    Task<IEnumerable<Order>> GetAllOrdersAsync();
    Task AddOrderAsync(Order order, string loggedBy);
    Task UpdateOrderAsync(Order order, string loggedBy);
    Task DeleteOrderAsync(Guid id, string loggedBy);
}
