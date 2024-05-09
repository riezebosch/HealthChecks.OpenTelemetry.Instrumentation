namespace SampleApi;

using System.Security.Cryptography;
using HealthChecks.ApplicationStatus.DependencyInjection;
using HealthChecks.OpenTelemetry.Instrumentation;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OpenTelemetry.Metrics;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddHealthChecks()
            .AddApplicationStatus()
            .AddProcessAllocatedMemoryHealthCheck(5)
            .AddDiskStorageHealthCheck(options => options.WithCheckAllDrives())
            .AddAsyncCheck("random", async () =>
            {
                var metadata = new Dictionary<string, object>
                {
                    { "tenant", "mytenant1" }
                };

                await Task.Delay(RandomNumberGenerator.GetInt32(100, 501));
                var rng = RandomNumberGenerator.GetInt32(100) % 3;

                switch (rng)
                {
                    case 0:
                        return HealthCheckResult.Unhealthy(data: metadata);

                    case 1:
                        return HealthCheckResult.Degraded(data: metadata);

                    default:
                        return HealthCheckResult.Healthy(data: metadata);
                }
            });

        builder.Services.Configure<HealthChecksInstrumentationOptions>(options =>
        {
            options.StatusGaugeName = "myapp.health";
            options.DurationGaugeName = "myapp.health.duration";
            options.IncludeHealthCheckMetadata = true;
        });

        builder.Services.AddOpenTelemetry()
            .WithMetrics(metricsBuilder => metricsBuilder
                .AddHealthChecksInstrumentation()
                .AddAspNetCoreInstrumentation()
                .AddPrometheusExporter()
                .AddConsoleExporter());

        var app = builder.Build();

        app.MapGet("/", () => Results.Redirect("/metrics"));

        app.UseHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        app.UseOpenTelemetryPrometheusScrapingEndpoint();

        app.Run();
    }
}