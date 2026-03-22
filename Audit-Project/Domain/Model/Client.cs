namespace Audit_Project.Domain;

/// <summary>
/// Represents a client entity with a name and company.
/// </summary>
[AuditableEntity("clients")]          // log → client_log (automático)
public class Client : BaseModel
{
    /// <summary>
    /// The name of the client.
    /// </summary>
    public string ClientName { get; set; }

    /// <summary>
    /// The name of the company associated with the client.
    /// </summary>
    public string CompanyName { get; set; }

    /// <summary>
    /// Initializes a new instance of the Client class.
    /// </summary>
    /// <param name="clientName">The client's name.</param>
    /// <param name="companyName">The company name.</param>
    public Client(string clientName, string companyName)
    {
        ClientName = clientName;
        CompanyName = companyName;
    }
}
