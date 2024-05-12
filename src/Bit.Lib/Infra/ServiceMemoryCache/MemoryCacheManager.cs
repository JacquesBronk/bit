using Bit.Lib.App;
using Bit.Lib.Common.Exception;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Bit.Log.Extensions;
using Microsoft.Extensions.Logging;

namespace Bit.Lib.Infra.ServiceMemoryCache;

public class MemoryCacheManager: ICacheManagerService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public MemoryCacheManager(IServiceScopeFactory scopeFactory, IMemoryCache memoryCache)
    {
        _scopeFactory = scopeFactory;
        Cache = memoryCache;
        RecreateCache();
    }
    
    public bool FlushCache()
    {
        RecreateCache();
        return true;
    }

    public Task<Result<bool>> TryGetValue(string key,  CancellationToken cancellationToken = default)
    {
        try
        {
            return Task.FromResult(Cache.TryGetValue(key, out var value) ? Result<bool>.Success(value is bool and true) : Result<bool>.Failure(new CannotFindKeyException(nameof(TryGetValue),$"{key} not found in mem-cache",[key])));
        }
        catch (Exception e)
        {
            
            return Task.FromResult(Result<bool>.Failure(e));
        }
        
    }

    public Task<Result<T>> Set<T>(string key, T value, MemoryCacheEntryOptions options, CancellationToken cancellationToken = default)
    {
        Cache.Set(key, value, options);
        return Task.FromResult(Result<T>.Success(value));
    }

    public Task<Result<bool>> Remove(string key, CancellationToken cancellationToken = default)
    {
        Cache.Remove(key);
        return Task.FromResult(Result<bool>.Success(true));
    }

    public IMemoryCache Cache { get; private set; }

    private void RecreateCache()
    {
        Cache.Dispose();
        using var scope = _scopeFactory.CreateScope();
        scope.Log<MemoryCacheManager>(LogLevel.Information,$"Recreating cache");
        Cache = new MemoryCache(new MemoryCacheOptions());
    }
}