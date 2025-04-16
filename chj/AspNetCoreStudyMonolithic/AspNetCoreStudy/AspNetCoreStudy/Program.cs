namespace AspNetCoreStudy;

using Microsoft.Extensions.Diagnostics.HealthChecks;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers();
        builder.Services.AddConfigures();
        builder.Services.AddSystemServices();
        builder.Services.AddOutGameServices();
        builder.Services.AddHostedServices();
        builder.Services.AddFilters();
        builder.Services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy());
        builder.Services.AddEndpointsApiExplorer();

        var app = builder.Build();
        app.AddMiddlewares();
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapHealthChecks("/health");
        app.MapControllers();
        app.Run();
    }
}
