using System.Diagnostics.Metrics;
using Bit.Lib.Infra.Instrumentation;

namespace Bit.Lib.Infra.Metrics;

public class HttpCommonMetrics
{
    public const string MeterName = "Bit.Api.Http";

    private readonly Counter<long> _weatherRequestCounter;
    private readonly Histogram<double> _weatherRequestDuration;
    private readonly Histogram<double> _weatherRequestSize;

    public HttpCommonMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create(MeterName);
        _weatherRequestCounter = meter.CreateCounter<long>(
            "bit.api.requests.count");

        _weatherRequestDuration = meter.CreateHistogram<double>(
            "bit.api.requests.duration",
            "ms");

        _weatherRequestSize = meter.CreateHistogram<double>(
            "bit.api.requests.size",
            "bytes");
    }

    public void IncreaseWeatherRequestCount()
    {
        _weatherRequestCounter.Add(1);
    }

    public TrackedRequestDuration MeasureRequestDuration()
    {
        return new TrackedRequestDuration(_weatherRequestDuration);
    }

    public TrackedRequestSize MeasureRequestSize(HttpContent httpContent)
    {
        return new TrackedRequestSize(_weatherRequestSize, httpContent);
    }
}



