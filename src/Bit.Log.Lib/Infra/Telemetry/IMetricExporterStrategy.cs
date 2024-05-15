using OpenTelemetry.Metrics;

namespace Bit.Log.Infra.Telemetry;

public interface IMetricExporterStrategy
{
    void ConfigureExporter(MeterProviderBuilder builder);
}