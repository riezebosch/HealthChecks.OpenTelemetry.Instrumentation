namespace HealthChecks.OpenTelemetry.Instrumentation;

/// <summary>
///     Options for <see cref="HealthChecksMetrics" /> instrumentation.
/// </summary>
public class HealthChecksInstrumentationOptions
{
    internal static readonly string HealthCheckDescription =
        "ASP.NET Core health check status (0 == Unhealthy, 0.5 == Degraded, 1 == Healthy)";

    internal static readonly string HealthCheckDurationDescription =
        "Shows duration of the health check execution in seconds";

    /// <summary>
    /// Gets or sets the name of the status gauge metric for health checks.
    /// </summary>
    /// <remarks>
    /// Reference <see href="https://github.com/open-telemetry/semantic-conventions/blob/v1.25.0/docs/general/metrics.md#metric-attributes">Metrics Semantic Conventions for Metrics attributes</see>.
    /// </remarks>
    public string StatusGaugeName { get; set; } = "aspnetcore.healthcheck";

    /// <summary>
    /// Gets or sets the name of the duration gauge metric for health checks.
    /// </summary>
    /// <remarks>
    /// Reference <see href="https://github.com/open-telemetry/semantic-conventions/blob/v1.25.0/docs/general/metrics.md#metric-attributes">Metrics Semantic Conventions for Metrics attributes</see>.
    /// </remarks>
    public string DurationGaugeName { get; set; } = "aspnetcore.healthcheck.duration";

    /// <summary>
    /// Gets or sets a value indicating whether health check metadata are added to health checks metrics.
    /// </summary>
    public bool IncludeHealthCheckMetadata { get; set; }
}