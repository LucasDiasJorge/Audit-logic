using Audit_Project.Repository;
using Audit_Project.Service;
using Audit_Project.Service.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Audit_Project;

/// <summary>
/// Extension methods for registering project repositories and services.
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Registers all repositories in the DI container.
    /// </summary>
    public static IServiceCollection AddProjectRepositories(this IServiceCollection services)
    {
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        return services;
    }

    /// <summary>
    /// Registers all services in the DI container.
    /// </summary>
    public static IServiceCollection AddProjectServices(this IServiceCollection services)
    {
        services.AddScoped<IClientService, ClientService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IOrderService, OrderService>();
        return services;
    }
}
