using Azure.Monitor.OpenTelemetry.Exporter;
using Bit.Log.Infra.Configuration;
using Microsoft.Extensions.Options;
using OpenTelemetry.Metrics;

namespace Bit.Log.Infra.Telemetry;

public class ApplicationInsightsExporterStrategy(IOptionsMonitor<OpenTelemetryOptions> optionsMonitor) : IMetricExporterStrategy
{
    public void ConfigureExporter(MeterProviderBuilder builder)
    {
        var aiOptions = optionsMonitor.CurrentValue.Traces.AzureMonitorExporterOptions;

        if (aiOptions != null)
        {
            if (string.IsNullOrEmpty(aiOptions.ConnectionString))
            {
                throw new ArgumentNullException(nameof(aiOptions.ConnectionString), "ConnectionString is required.");
            }

            builder.AddAzureMonitorMetricExporter(options =>
            {
                options.ConnectionString = aiOptions.ConnectionString;
            });
        }
        else
        {
            throw new ArgumentNullException(nameof(aiOptions), "AzureMonitorExporterOptions is required.");
        }
    }
}