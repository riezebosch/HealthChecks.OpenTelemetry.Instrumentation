# HealthChecks Instrumentation for OpenTelemetry .NET

[![Nuget (with prereleases)](https://img.shields.io/nuget/v/HealthChecks.OpenTelemetry.Instrumentation)](https://www.nuget.org/packages/HealthChecks.OpenTelemetry.Instrumentation)
[![NuGet download count badge](https://img.shields.io/nuget/dt/HealthChecks.OpenTelemetry.Instrumentation)](https://www.nuget.org/packages/HealthChecks.OpenTelemetry.Instrumentation)
[![feedz.io](https://img.shields.io/badge/endpoint.svg?url=https%3A%2F%2Ff.feedz.io%2Fgowon%2Fpre-release%2Fshield%2FHealthChecks.OpenTelemetry.Instrumentation%2Flatest)](https://f.feedz.io/gowon/pre-release/packages/HealthChecks.OpenTelemetry.Instrumentation/latest/download)

This is an [Instrumentation Library](https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/glossary.md#instrumentation-library), which instruments [Microsoft.Extensions.Diagnostics.HealthChecks](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks) and collect metrics about the application health checks.

> [!CAUTION]
> This component is based on the `v1.25` OpenTelemetry semantic conventions for [metrics](https://github.com/open-telemetry/semantic-conventions/blob/v1.25.0/docs/general/metrics.md). These conventions are [Mixed](https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/document-status.md), and hence, this package is a [pre-release](https://github.com/open-telemetry/opentelemetry-dotnet/blob/main/VERSIONING.md#pre-releases). Until a [stable version](https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/telemetry-stability.md) is released, there can be breaking changes. You can track the progress from [milestones](https://github.com/gowon/HealthChecks.OpenTelemetry.Instrumentation/milestone/1).

## Supported .NET Versions

This package targets [`netstandard2.0`](https://docs.microsoft.com/dotnet/standard/net-standard#net-implementation-support) and hence can be used in any .NET versions implementing `netstandard2.0`.

## Steps to enable HealthChecks.OpenTelemetry.Instrumentation

### Step 1: Install package

Add a reference to the [`HealthChecks.OpenTelemetry.Instrumentation`](https://www.nuget.org/packages/HealthChecks.OpenTelemetry.Instrumentation) package:

```shell
dotnet add package HealthChecks.OpenTelemetry.Instrumentation
```

### Step 2: Enable HealthChecks Instrumentation

HealthChecks instrumentation should be enabled at application startup using the `AddHealthChecksInstrumentation` extension on `MeterProviderBuilder`. The following example demonstrates adding HealthChecks instrumentation to a console application. This example also sets up the OpenTelemetry Console exporter, which requires adding the package [`OpenTelemetry.Exporter.Console`](https://github.com/open-telemetry/opentelemetry-dotnet/blob/main/src/OpenTelemetry.Exporter.Console/README.md) to the application:

```csharp
using OpenTelemetry;
using OpenTelemetry.Metrics;

public class Program
{
    public static void Main(string[] args)
    {
        using var meterProvider = Sdk.CreateMeterProviderBuilder()
            .AddHealthChecksInstrumentation()
            .AddConsoleExporter()
            .Build();
    }
}
```

For an ASP.NET Core application, adding instrumentation is typically done in the `ConfigureServices` of your `Startup` class. Refer to documentation for [OpenTelemetry.Instrumentation.AspNetCore](https://github.com/open-telemetry/opentelemetry-dotnet/blob/main/src/OpenTelemetry.Instrumentation.AspNetCore/README.md).

Refer to [Program.cs](samples/SampleApi/Program.cs) for a complete demo.

### Advanced configuration

This instrumentation can be configured to change the default behavior by using `HealthChecksInstrumentationOptions`:

```csharp
using var meterProvider = Sdk.CreateMeterProviderBuilder()
    .AddHealthChecksInstrumentation(options =>
    {
        options.StatusGaugeName = "myapp.health";
        options.DurationGaugeName = "myapp.health.duration";
        options.IncludeHealthCheckMetadata = true;
    })
    .AddConsoleExporter()
    .Build();
```

When used with [`OpenTelemetry.Extensions.Hosting`](https://github.com/open-telemetry/opentelemetry-dotnet/blob/main/src/OpenTelemetry.Extensions.Hosting/README.md), all configurations to `HealthChecksInstrumentationOptions` can be done in the `ConfigureServices` method of you applications `Startup` class as shown below:

```csharp
services.Configure<HealthChecksInstrumentationOptions>(options =>
{
    options.StatusGaugeName = "myapp.health";
    options.DurationGaugeName = "myapp.health.duration";
    options.IncludeHealthCheckMetadata = true;
});

services.AddOpenTelemetry()
    .WithMetrics(builder => builder
        .AddHealthChecksInstrumentation()
        .AddConsoleExporter());
```

## Metrics

### aspnetcore.healthcheck

Gets the health status of the component that was checked, converted to double value (0 == Unhealthy, 0.5 == Degraded, 1 == Healthy).

| Units | Instrument Type | Value Type | Attribute Key(s) | Attribute Values |
|-|-|-|-|-|
| `status` | ObservableGauge | `Double`    | name       | name of each executed health check |

The API used to retrieve the value is:

- [HealthReportEntry.Status](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.diagnostics.healthchecks.healthreportentry.status): Gets the health status of the component that was checked.

### aspnetcore.healthcheck.duration

Gets the health check execution duration.

| Units | Instrument Type | Value Type | Attribute Key(s) | Attribute Values |
|-|-|-|-|-|
| `seconds` | ObservableGauge | `Double`    | name       | name of each executed health check |

The API used to retrieve the value is:

- [HealthReportEntry.Duration](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.diagnostics.healthchecks.healthreportentry.duration): Gets the health check execution duration.

## References

- [OpenTelemetry Project](https://opentelemetry.io/)
