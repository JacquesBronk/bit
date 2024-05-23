using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics.Metrics;
using StackExchange.Redis;

namespace Bit.Lib.Infra.Instrumentation.Jobs
{
    public class HistogramRunner : BackgroundService
    {
        private readonly ILogger<HistogramRunner> _logger;
        private readonly Histogram<double> _histogram;
        private readonly ConnectionMultiplexer _connectionMultiplexer;
        private readonly TimeSpan _checkInterval;

        public HistogramRunner(ILogger<HistogramRunner> logger, Histogram<double> histogram, ConnectionMultiplexer connectionMultiplexer, TimeSpan checkInterval)
        {
            _logger = logger;
            _histogram = histogram;
            _connectionMultiplexer = connectionMultiplexer;
            _checkInterval = checkInterval;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug("HistogramRunner started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var trackedLatency = new TrackedRedisLatency(_histogram, _connectionMultiplexer))
                    {
                        // The latency is recorded upon disposing trackedLatency
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while checking Redis latency");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("HistogramRunner stopping");
        }
    }
}