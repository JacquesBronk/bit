using System.Diagnostics.Metrics;
using Bit.Lib.Infra.Repositories;

namespace Bit.Lib.Infra.Metrics;

public class RedisKeyCount
{
    private readonly FlagRepository _flagRepository;

    public RedisKeyCount(FlagRepository flagRepository)
    {
        var meter = new Meter("KeyCountMeter", "1.0.0");
        _flagRepository = flagRepository;

        meter.CreateObservableCounter<int>("keycount", () => new Measurement<int>(FetchKeyCount()));
    }

    private int FetchKeyCount()
    {
        var keyCountResult =  _flagRepository.RedisKeyCount(CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();

        return keyCountResult.IsSuccess ? keyCountResult.Value : 0;
    }
}