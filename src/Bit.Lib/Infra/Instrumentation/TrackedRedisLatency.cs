using System.Diagnostics;
using System.Diagnostics.Metrics;
using StackExchange.Redis;

namespace Bit.Lib.Infra.Instrumentation;

public class TrackedRedisLatency : IDisposable
{
    private readonly Histogram<double> _histogram;
    private readonly Stopwatch _stopwatch;

    public TrackedRedisLatency(Histogram<double> histogram, ConnectionMultiplexer connectionMultiplexer)
    {
        _histogram = histogram;
        _stopwatch = Stopwatch.StartNew();
        connectionMultiplexer.GetDatabase().Ping();
        _stopwatch.Stop();
    }

    public void Dispose()
    {
        _histogram.Record(_stopwatch.Elapsed.TotalMilliseconds);
    }
}