using System.Data;
using Dapper;

namespace Audit_Project.Database;

/// <summary>
/// Provides methods to test database connectivity.
/// </summary>
public class DatabaseTest(IDbConnection connection)
{
    /// <summary>
    /// Checks if the database connection is healthy by executing a simple query.
    /// </summary>
    /// <returns>True if the connection is successful; otherwise, false.</returns>
    public bool HealthCheck()
    {
        // Implement a simple health check logic, e.g., check database connectivity
        try
        {
            connection.Open();
            string query = "SELECT version();"; // Simple query to test database connection
            string queryResult = connection.QuerySingle<string>(query);
            Console.WriteLine($"Database version: {queryResult}");
            return true; // Database connection successful
        }
        catch
        {
            return false; // Database connection failed
        }
        finally
        {
            connection.Close();
        }
    }
}
