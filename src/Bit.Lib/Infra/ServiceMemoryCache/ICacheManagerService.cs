using Bit.Lib.App;
using Microsoft.Extensions.Caching.Memory;

namespace Bit.Lib.Infra.ServiceMemoryCache;

public interface ICacheManagerService
{
    IMemoryCache Cache { get; }
    bool FlushCache();

    Task<Result<bool>> TryGetValue(string key, CancellationToken cancellationToken = default);
    Task<Result<T>> Set<T>(string key, T value, MemoryCacheEntryOptions options, CancellationToken cancellationToken = default);
    Task<Result<bool>> Remove(string key, CancellationToken cancellationToken = default);
    
}