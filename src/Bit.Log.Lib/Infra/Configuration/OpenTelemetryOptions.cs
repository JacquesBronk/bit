namespace Bit.Log.Infra.Configuration;

public class OpenTelemetryOptions
{
    public required TracesOptions Traces { get; set; }
    public MetricsOptions? Metrics { get; set; }
}