using System.Diagnostics.Metrics;
using Bit.Log.Infra.Configuration;
using Bit.Log.Infra.Telemetry.Metrics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Bit.Log.Infra.Jobs;

public class HistogramRunner : BackgroundService
{
    private readonly IReadOnlyList<Histogram<double>> _histograms;
    private Timer? _timer;
    private readonly ILogger<HistogramRunner> _logger;

    public HistogramRunner(HistogramBuilder histogramBuilder, IConfiguration configuration, ILogger<HistogramRunner> logger)
    {
        _logger = logger;
        _histograms = histogramBuilder
            .AddHistogramsFromConfiguration(configuration.GetSection("Metrics").Get<MetricsOptions>()).Build();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _timer = new Timer(RecordHistogramValues, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        return Task.CompletedTask;
    }

    private void RecordHistogramValues(object? state)
    {
        try
        {
            var random = new Random();

            foreach (var histogram in _histograms)
            {
                double value = random.NextDouble() * 1000;
                histogram.Record(value);
            }
            _logger.LogInformation("{Message}", "Histograms recorded successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(0, ex, "An error occurred while recording histogram values");
            throw new Exception("RecordHistogramValues", new Exception("Failed to record histogram values"));
        }
    }

    public override void Dispose()
    {
        _timer?.Dispose();
        base.Dispose();
    }
}