using System.Diagnostics.Metrics;
using Bit.Log.Common.Exception;
using Bit.Log.Infra.Configuration;
using Bit.Log.Infra.Telemetry.Metrics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Bit.Log.Infra.Jobs;

public class HistogramRunner(HistogramBuilder histogramBuilder, IConfiguration configuration, ILogger<HistogramRunner> logger) : BackgroundService
{
    private readonly IReadOnlyList<Histogram<double>> _histograms = histogramBuilder
        .AddHistogramsFromConfiguration(configuration.GetSection("Metrics").Get<MetricsOptions>()).Build();
    private Timer? _timer;

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
            logger.LogInformation("{Message}", "Histograms recorded successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(0, ex, "An error occurred while recording histogram values");
            string? stateException = string.IsNullOrWhiteSpace(state?.ToString()) ? string.Empty : state.ToString();
            throw new CannotRecordHistogramValuesException(nameof(RecordHistogramValues),"An error occurred while recording histogram values",new[] {stateException ?? string.Empty}, ex);
        }
    }

    public override void Dispose()
    {
        _timer?.Dispose();
        base.Dispose();
    }
}