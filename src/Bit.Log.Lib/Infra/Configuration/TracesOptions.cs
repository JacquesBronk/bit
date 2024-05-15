namespace Bit.Log.Infra.Configuration;

public class TracesOptions
{
    public required string Sampler { get; set; }
    public required string Exporter { get; set; }
    public OtlpExporterOptions? OtlpExporterOptions { get; set; }
    public AzureMonitorExporterOptions? AzureMonitorExporterOptions { get; set; }
}