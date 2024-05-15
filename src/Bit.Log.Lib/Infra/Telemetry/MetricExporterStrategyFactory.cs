using Bit.Log.Infra.Configuration;
using Microsoft.Extensions.Options;
using static Bit.Log.Infra.Telemetry.MetricExporterType;

namespace Bit.Log.Infra.Telemetry;

public class MetricExporterStrategyFactory(IOptionsMonitor<OpenTelemetryOptions> optionsMonitor)
{
    private readonly IOptionsMonitor<OpenTelemetryOptions> _optionsMonitor = optionsMonitor;

    public IMetricExporterStrategy CreateExporterStrategy(string exporterType)
    {
        return exporterType switch
        {
            Prometheus => new OpenTelemetryCollectorExporterStrategy(_optionsMonitor),
            Jaeger => new OpenTelemetryCollectorExporterStrategy(_optionsMonitor),
            ApplicationInsights => new ApplicationInsightsExporterStrategy(_optionsMonitor),
            OpenTelemetryCollector => new OpenTelemetryCollectorExporterStrategy(_optionsMonitor),
            _ => throw new ArgumentException("Invalid exporter type")
        };
    }
}