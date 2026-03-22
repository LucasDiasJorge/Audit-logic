using Audit_Project.Database;
using Audit_Project.Service;
using MySqlConnector;
using System.Data;

namespace Audit_Project;

/// <summary>
/// Configures application services and database connection.
/// </summary>
public class Startup()
{
    /// <summary>
    /// Configures dependency injection and checks database connectivity.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">The application configuration.</param>
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthorization();
        services.AddOpenApi();
        services.AddScoped<IDbConnection>(_ => new MySqlConnection(configuration["MySqlConnection"]));
        services.AddScoped<DatabaseTest>();

        // Build a temporary provider to resolve DatabaseTest for health check
        using (ServiceProvider provider = services.BuildServiceProvider())
        {
            DatabaseTest dbTest = provider.GetRequiredService<DatabaseTest>();
            if (!dbTest.HealthCheck())
            {
                throw new Exception("Database connection failed.");
            }
        }

        DependencyInjectionExtensions.AddProjectServices(services);
        DependencyInjectionExtensions.AddProjectRepositories(services);
    }
}
