namespace Audit_Project.Domain;

/// <summary>
/// Represents an order entity, containing a client and a list of products.
/// </summary>
[AuditableEntity("orders")]          // log → order_log (automático)
public class Order : BaseModel
{
    /// <summary>
    /// The client associated with the order.
    /// </summary>
    public required Client Client { get; set; }

    /// <summary>
    /// The list of products in the order.
    /// </summary>
    public List<Product> Products { get; set; } = new List<Product>();

    /// <summary>
    /// Initializes a new instance of the Order class.
    /// </summary>
    /// <param name="client">The client for the order.</param>
    public Order(Client client)
    {
        Client = client;
    }

    /// <summary>
    /// Adds a product to the order and updates the timestamp.
    /// </summary>
    /// <param name="product">The product to add.</param>
    public void AddProduct(Product product)
    {
        Products.Add(product);
        UpdatedAt = DateTime.UtcNow;
    }

}
