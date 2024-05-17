using Bit.Lib.App;
using Bit.Lib.Infra.ServiceMemoryCache;
using Bit.Log.Common.Exception;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Bit.Lib.Infra.Repositories;

public class FlagRepository(ConnectionMultiplexer connectionMultiplexer, ICacheManagerService memoryCache, ILogger<FlagRepository> logger) : IFlagRepository
{
    private readonly IConnectionMultiplexer _connectionMultiplexer = connectionMultiplexer;

    public async Task<Result<bool>> IsEnabled(string flagName, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"f_{flagName.ToLowerInvariant()}";

        bool isEnabled = false;
        try
        {
            isEnabled = await TryGetFlagFromMemoryCache(cacheKey, cancellationToken);
        }
        catch (Exception)
        {
            var memoryCacheDataException = new CannotFindKeyException(nameof(IsEnabled), $"unable to find entry for {flagName} in memory cache", [flagName]);
            logger.LogTrace(memoryCacheDataException, "unable to find entry for {FlagName} in memory cache", flagName);
        }

        if (isEnabled)
        {
            return Result<bool>.Success(isEnabled);
        }

        try
        {
            isEnabled = await TryGetFromRedis(cacheKey, cancellationToken);
            return Result<bool>.Success(isEnabled);
        }
        catch (Exception ex)
        {
            var redisException = new CannotFindKeyException(nameof(IsEnabled), $"unable to find entry for {flagName} in redis", [flagName], ex);
            logger.LogError(redisException, "unable to find entry for {FlagName}", flagName);
            return Result<bool>.Failure(redisException);
        }
    }

    public async Task<Result<(string flagName, bool IsEnabled)>> FetchFlagStatus(string flagName, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"f_{flagName.ToLowerInvariant()}";

        bool isEnabled = false;
        try
        {
            isEnabled = await TryGetFlagFromMemoryCache(cacheKey, cancellationToken);
        }
        catch (Exception)
        {
            var memoryCacheDataException = new CannotFindKeyException(nameof(FetchFlagStatus), $"unable to find entry for {flagName} in memory cache", [flagName]);
            logger.LogTrace(memoryCacheDataException, "unable to find entry for {FlagName} in memory cache", flagName);
        }

        if (isEnabled)
        {
            return Result<(string flagName, bool IsEnabled)>.Success(new (flagName, isEnabled));
        }

        try
        {
            isEnabled = await TryGetFromRedis(cacheKey, cancellationToken);
            return Result<(string flagName, bool IsEnabled)>.Success(new (flagName, isEnabled));
        }
        catch (Exception ex)
        {
            var redisException = new CannotFindKeyException(nameof(FetchFlagStatus), $"unable to find entry for {flagName}", [flagName], ex);
            logger.LogError(redisException, "unable to find entry for {FlagName}", flagName);
            return Result<(string flagName, bool IsEnabled)>.Failure(redisException);
        }
    }

    public async Task<Result<(string flagName, bool IsEnabled)>> UpdateFlag(string flagName, bool isEnabled, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"f_{flagName.ToLowerInvariant()}";
        try
        {
            await memoryCache.Remove(cacheKey, cancellationToken);
        }
        catch (Exception e)
        {
            var memoryCacheUpdateException = new CannotUpdateKeyException(nameof(UpdateFlag), $"unable to remove entry to update for {flagName} in memory cache", [flagName], e);
            logger.LogTrace(memoryCacheUpdateException, "unable to find entry for {FlagName} in memory cache", flagName);
        }

        try
        {
            isEnabled = await TrySetFlagOnRedis(cacheKey, isEnabled, cancellationToken);
            
            return Result<(string flagName, bool IsEnabled)>.Success(new (flagName, isEnabled));
        }
        catch (Exception e)
        {
            var redisException = new CannotUpdateKeyException(nameof(UpdateFlag), $"unable to update entry for {flagName} in redis", [flagName], e);
            logger.LogError(redisException, "unable to update entry for {FlagName}", flagName);
            return Result<(string flagName, bool IsEnabled)>.Failure(redisException);
        }
    }

    public async Task<Result<(string flagName, bool IsEnabled)>> CreateFlag(string flagName, bool isEnabled, CancellationToken cancellationToken = default)
    {
        //create flag in redis
        var cacheKey = $"f_{flagName.ToLowerInvariant()}";
        try
        {
            var db = _connectionMultiplexer.GetDatabase();
            await db.StringSetAsync(cacheKey, isEnabled);
            isEnabled = await TryGetFromRedis(cacheKey, cancellationToken);
            return Result<(string flagName, bool IsEnabled)>.Success(new (flagName, isEnabled));
        }
        catch (Exception e)
        {
            var redisException = new CannotCreateFlagException(nameof(CreateFlag), "unable to handle create request for redis cache", [], e);
            logger.LogError(redisException, "Error creating flag");
            return Result<(string flagName, bool IsEnabled)>.Failure(redisException);
        }
    }

    public async Task<Result<(string flagName, bool IsEnabled)>> DeleteFlag(string flagName, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"f_{flagName.ToLowerInvariant()}";
        try
        {
            var db = _connectionMultiplexer.GetDatabase();
            await db.KeyDeleteAsync(cacheKey);
            await memoryCache.Remove(cacheKey, cancellationToken);
            return Result<(string flagName, bool IsEnabled)>.Success(new (flagName, false));
        }
        catch (Exception e)
        {
            var redisException = new CannotDeleteFlagException(nameof(DeleteFlag), "unable to handle delete request for redis cache", [], e);
            logger.LogError(redisException, "Error deleting flag");
            return Result<(string flagName, bool IsEnabled)>.Failure(redisException);
        }
    }

    public async Task<Result<List<KeyValuePair<string, bool>>>> FetchAllFlags(CancellationToken cancellationToken = default)
    {
        var allFlags = new List<KeyValuePair<string, bool>>();

        
        var endpoints = _connectionMultiplexer.GetEndPoints(true);
        foreach (var endpoint in endpoints)
        {
            var server = _connectionMultiplexer.GetServer(endpoint);
            var keys = server.Keys(pattern: "f_*");
            var db = _connectionMultiplexer.GetDatabase();
            foreach (var key in keys)
            {
                var value = await db.StringGetAsync(key);
                allFlags.Add(new KeyValuePair<string, bool>($"redis_{key}", (bool)value));
            }
        }

        var memCacheKeys = allFlags.Select(x => x.Key.Replace("redis_", ""));
        foreach (var key in memCacheKeys)
        {
            try
            {
                var isEnabled = await TryGetFlagFromMemoryCache(key, cancellationToken);
                allFlags.Add(new KeyValuePair<string, bool>($"memory_{key}", isEnabled));
            }
            catch (Exception)
            {
                allFlags.Add(new KeyValuePair<string, bool>($"missing_memory_{key}", false));
            }
        }

        return Result<List<KeyValuePair<string, bool>>>.Success(allFlags);
    }

    public async Task<Result<bool>> FlushCache(CancellationToken cancellationToken = default)
    {
        try
        {
            memoryCache.FlushCache();
        }
        catch (Exception e)
        {
            var memoryCacheFlushException = new CannotFlushCacheException(nameof(FlushCache), "unable to handle flush request for memory cache", [], e);
            logger.LogTrace(memoryCacheFlushException, "Error flushing cache");
        }


        try
        {
            var endpoints = _connectionMultiplexer.GetEndPoints(true);
            foreach (var endpoint in endpoints)
            {
                var server = _connectionMultiplexer.GetServer(endpoint);
                if (!server.IsReplica)
                {
                    await server.FlushDatabaseAsync();
                }
            }

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            var redisException = new CannotFlushCacheException(nameof(FlushCache), "unable to handle flush request for redis cache", [], ex);
            logger.LogError(redisException, "Error flushing cache");
            return Result<bool>.Failure(redisException);
        }
    }

    private async Task<bool> TrySetFlagOnRedis(string flagName, bool isEnabled, CancellationToken cancellationToken = default)
    {
        memoryCache.FlushCache();
        
        var cacheKey = $"f_{flagName.ToLowerInvariant()}";
        var db = _connectionMultiplexer.GetDatabase();
        bool keyExists = await db.KeyExistsAsync(cacheKey);
        if (!keyExists)
        {
            throw new CannotFindKeyException(nameof(TrySetFlagOnRedis), $"Key {cacheKey} not found in redis", [flagName, isEnabled.ToString()]);
        }

        await db.StringSetAsync(cacheKey, isEnabled);
        return await TryGetFromRedis(flagName, cancellationToken);
    }

    private async Task<bool> TryGetFlagFromMemoryCache(string flagName, CancellationToken cancellationToken = default)
    {
        var cacheKey = flagName.StartsWith("f_") ? flagName.ToLowerInvariant() : $"f_{flagName.ToLowerInvariant()}";
        bool isEnabled = await memoryCache.TryGetValue(cacheKey, cancellationToken);
        if (isEnabled)
        {
            return Result<bool>.Success(isEnabled);
        }

        return false;
    }

    private async Task<bool> TryGetFromRedis(string cacheKey, CancellationToken cancellationToken = default)
    {
        var db = _connectionMultiplexer.GetDatabase();

        var redisValue = await db.StringGetAsync(cacheKey);

        var isEnabled = redisValue.HasValue && (bool)redisValue;

        if (!isEnabled) return isEnabled;

        await memoryCache.Set(cacheKey, isEnabled, MemoryCacheOptions, cancellationToken);

        return isEnabled;
    }

    public Task<Result<int>> RedisKeyCount(CancellationToken cancellationToken = default)
    {
        try
        {
            var endpoints = _connectionMultiplexer.GetEndPoints(true);
            int totalKeys = endpoints.Select(endpoint => _connectionMultiplexer.GetServer(endpoint)).Where(server => !server.IsReplica).Sum(server => server.Keys(pattern: "f_*").Count());
            return Task.FromResult(Result<int>.Success(totalKeys));
        }
        catch (Exception e)
        {
            var redisException = new CannotGetKeyCountException(nameof(RedisKeyCount), "unable to get key count from redis", [], e);
            logger.LogError(redisException, "Error getting key count");
            return Task.FromResult(Result<int>.Failure(redisException));
        }

    }
    
    MemoryCacheEntryOptions MemoryCacheOptions => new MemoryCacheEntryOptions()
        .SetSlidingExpiration(TimeSpan.FromMinutes(5))
        .RegisterPostEvictionCallback((key, value, reason, state) => { logger.LogTrace("Evicted key: {Key}, {Value} reason: {Reason} state: {State}", key, value, reason, state); });
}