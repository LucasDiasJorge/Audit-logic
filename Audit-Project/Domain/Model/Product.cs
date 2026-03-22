namespace Audit_Project.Domain;

/// <summary>
/// Represents a product entity with a name and price.
/// </summary>
[AuditableEntity("products")]          // log → product_log (automático)
public class Product : BaseModel
{
    /// <summary>
    /// The name of the product.
    /// </summary>
    public string ProductName { get; set; }

    /// <summary>
    /// The price of the product.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Initializes a new instance of the Product class.
    /// </summary>
    /// <param name="productName">The product's name.</param>
    /// <param name="price">The product's price.</param>
    public Product(string productName, decimal price)
    {
        ProductName = productName;
        Price = price;
    }
}
