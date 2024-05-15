namespace Bit.Log.Infra.Configuration;

public class OtlpExporterOptions
{
    public required string Endpoint { get; init; }
    public required string Protocol { get; init; }
}