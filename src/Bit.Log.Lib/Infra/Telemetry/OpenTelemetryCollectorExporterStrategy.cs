using Bit.Log.Infra.Configuration;
using Microsoft.Extensions.Options;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;

namespace Bit.Log.Infra.Telemetry;

public class OpenTelemetryCollectorExporterStrategy(IOptionsMonitor<OpenTelemetryOptions> optionsMonitor) : IMetricExporterStrategy
{
    public void ConfigureExporter(MeterProviderBuilder builder)
    {
        var otlpOptions = optionsMonitor.CurrentValue.Traces.OtlpExporterOptions;

        if (otlpOptions == null)
        {
            return;
        }

        builder.AddOtlpExporter(options =>
        {
            options.Endpoint = new Uri(otlpOptions.Endpoint);
            options.Protocol = otlpOptions.Protocol switch
            {
                "grpc" => OtlpExportProtocol.Grpc,
                "http/protobuf" => OtlpExportProtocol.HttpProtobuf,
                _ => throw new ArgumentException("Invalid protocol specified")
            };
        });
    }
}