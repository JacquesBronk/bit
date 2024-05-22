using Bit.Lib.Common.CloudEvent;
using Bit.Lib.Infra.Repositories;
using CloudNative.CloudEvents;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Bit.Lib.Domain.Service;

public class FlagService(IFlagRepository flagRepository, IMemoryCache memoryCache, ILogger<FlagService> logger) : IFlagService
{
    public async Task<CloudEvent> GetFlag(string flagName, CancellationToken cancellationToken = default)
    {
        var flagStatus = await flagRepository.FetchFlagStatus(flagName, cancellationToken);

        flagStatus.OnFailure(_ => logger.LogError("{ExceptionMessage}", flagStatus.ToString()));
        
        return flagStatus.ToHasValueEvent();
    }

    public async Task<CloudEvent> IsFlagEnabled(string flagName, CancellationToken cancellationToken = default)
    {
        var isFlagEnabled = await flagRepository.IsEnabled(flagName, cancellationToken);

        isFlagEnabled.OnFailure(action =>
        {
            logger.LogError("{ExceptionMessage}", isFlagEnabled.ToString());
            isFlagEnabled.GetValueOrThrow();
            Task.FromResult(action);
        });

        if (isFlagEnabled.Value.Equals(false))
        {
            return (flagName, isFlagEnabled.Value).ToIsDisabledEvent();
        }

        return (flagName, isFlagEnabled.Value).ToIsEnabledEvent();
    }

    public async Task<CloudEvent> UpdateFlag(string flagName, bool isEnabled, CancellationToken cancellationToken = default)
    {
        var updateResult = await flagRepository.UpdateFlag(flagName, isEnabled, cancellationToken);

        updateResult.OnFailure(_ => logger.LogError("{ExceptionMessage}", updateResult.ToString()));

        return updateResult.ToFlagUpdatedCloudEvent();
    }

    public async Task<CloudEvent> CreateFlag(string flagName, bool isEnabled, CancellationToken cancellationToken = default)
    {
        var createResult = await flagRepository.CreateFlag(flagName, isEnabled, cancellationToken);

        createResult.OnFailure(_ => logger.LogError("{ExceptionMessage}", createResult.ToString()));

        return createResult.ToFlagCreatedCloudEvent();
    }

    public async Task<CloudEvent> DeleteFlag(string flagName, CancellationToken cancellationToken = default)
    {
        var deleteResult = await flagRepository.DeleteFlag(flagName, cancellationToken);

        deleteResult.OnFailure(_ => logger.LogError("{ExceptionMessage}", deleteResult.ToString()));

        return deleteResult.ToFlagDeletedCloudEvent();
    }

    public async Task<CloudEvent> FlushCache(CancellationToken cancellationToken = default)
    {
        var flushResult = await flagRepository.FlushCache(cancellationToken);
        
        flushResult.OnFailure(_ => logger.LogError("{ExceptionMessage}", flushResult.ToString()));
        
        return flushResult.ToCacheFlushedCloudEvent();
    }

    public Task<CloudEvent> GetMemCacheStats(string flagName, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(memoryCache.GetCurrentStatistics().ToMemCacheStatsEvent());
    }

    public async Task<CloudEvent> GetRedisCacheStats(string flagName, CancellationToken cancellationToken = default)
    {
        var redisStats = await flagRepository.RedisKeyCount(cancellationToken);
        redisStats.OnFailure(_ => logger.LogError("{ExceptionMessage}", redisStats.ToString()));

        return redisStats.ToRedisStatsEvent();
    }
}