using System.Diagnostics.Metrics;

namespace Bit.Lib.Infra.Instrumentation;

public class TrackedRequestDuration(Histogram<double> histogram) : IDisposable
{
    private readonly long _requestStartTime = TimeProvider.System.GetTimestamp();

    public void Dispose()
    {
        var elapsed = TimeProvider.System.GetElapsedTime(_requestStartTime);
        histogram.Record(elapsed.TotalMilliseconds);
    }
}