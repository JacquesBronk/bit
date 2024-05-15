namespace Bit.Log.Infra.Configuration;

public class HistogramConfig
{
    public required string Name { get; init; }
    public required string Unit { get; init; }
    public required string Description { get; init; }
}