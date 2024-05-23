using System.Diagnostics.Metrics;

namespace Bit.Lib.Infra.Instrumentation;

public class TrackedRequestSize(Histogram<double> histogram, HttpContent httpContent) : IDisposable
{
    public void Dispose()
    {
        var contentLength = httpContent.Headers.ContentLength ?? 0;
        histogram.Record(contentLength);
    }
}