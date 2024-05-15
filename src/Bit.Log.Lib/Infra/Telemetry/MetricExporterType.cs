namespace Bit.Log.Infra.Telemetry;

public static class MetricExporterType
{
    public const string Prometheus = "Prometheus";
    public const string Jaeger = "Jaeger";
    public const string ApplicationInsights = "ApplicationInsights";
    public const string OpenTelemetryCollector = "OpenTelemetryCollector";
}