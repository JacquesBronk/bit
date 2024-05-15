using System.Diagnostics.Metrics;
using Bit.Log.Infra.Configuration;

namespace Bit.Log.Infra.Telemetry.Metrics;

public class HistogramBuilder(Meter meter)
{
    private readonly List<Histogram<double>> _histograms = new();

    public HistogramBuilder AddHistogramsFromConfiguration(MetricsOptions? configuration)
    {
        if (configuration?.Histograms == null)
        {
            return this;
        }
   
        if (configuration.Histograms == null)
        {
            return this;
        }

        var index = 0;
        for (; index < configuration.Histograms.Count; index++)
        {
            var config = configuration.Histograms[index];
            var histogram = meter.CreateHistogram<double>(config.Name, config.Unit, config.Description);
            _histograms.Add(histogram);
        }

        return this;
    }

    public IReadOnlyList<Histogram<double>> Build() => _histograms;
}