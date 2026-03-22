
namespace Audit_Project;

/// <summary>
/// Entry point for the Audit Project application.
/// </summary>
public class Program
{
    /// <summary>
    /// Main method. Configures and runs the web application.
    /// </summary>
    /// <param name="args">Command-line arguments.</param>
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        builder.Services.AddAuthorization();
        builder.Services.AddOpenApi();

        Startup startup = new Startup();
        startup.ConfigureServices(builder.Services, builder.Configuration);

        WebApplication app = builder.Build();
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.Run();
    }
}
