namespace Bit.Log.Infra.Telemetry.Metrics;

public class HistogramConfig
{
    public HistogramConfig()
    {
        
    }
    public HistogramConfig(string name, string unit, string description)
    {
        Name = name;
        Unit = unit;
        Description = description;
    }

    public required string Name { get; init; }
    public required string Unit { get; init; }
    public required string Description { get; init; }
}