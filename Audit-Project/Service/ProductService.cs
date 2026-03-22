using Audit_Project.Domain;
using Audit_Project.Repository;
using Audit_Project.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Audit_Project.Service;

/// <summary>
/// Implements business logic for Product entities.
/// </summary>
public class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }
    public Task<Product> GetProductByIdAsync(Guid id) => _repository.GetByIdAsync(id);
    public Task<IEnumerable<Product>> GetAllProductsAsync() => _repository.GetAllAsync();
    public Task AddProductAsync(Product product) => _repository.AddAsync(product);
    public Task UpdateProductAsync(Product product) => _repository.UpdateAsync(product);
    public Task DeleteProductAsync(Guid id) => _repository.DeleteAsync(id);
}
