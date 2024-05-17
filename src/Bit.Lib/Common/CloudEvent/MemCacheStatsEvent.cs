using Bit.Log.Common;
using Microsoft.Extensions.Caching.Memory;

namespace Bit.Lib.Common.CloudEvent;

public static class MemCacheStatsEvent
{
    public static CloudNative.CloudEvents.CloudEvent ToMemCacheStatsEvent(this MemoryCacheStatistics? result)
    {
        if (result is null)
        {
            return new CloudNative.CloudEvents.CloudEvent
            {
                Id = Guid.NewGuid().ToString("N"),
                Subject = $"{EventCodes.FlagMemoryCacheStatistics}",
                Source = new Uri("https://bit.io/mem-cache-stats"),
                Time = DateTime.UtcNow,
                DataSchema = new Uri("https://bit.io/schemas/mem-cache-stats.json"),
                DataContentType = DataContentTypeConstants.Json,
                Type = ExceptionCodes.Infrastructure.CannotGetMemoryCacheStatistics,
                Data = new
                {
                    result
                }
            };
        }
        return new CloudNative.CloudEvents.CloudEvent
        {
            Id = Guid.NewGuid().ToString("N"),
            Subject = $"{EventCodes.FlagMemoryCacheStatistics}",
            Time = DateTime.UtcNow,
            Source = new Uri("https://bit.io/mem-cache-stats"),
            DataContentType = DataContentTypeConstants.Json,
            Type = EventCodes.FlagMemoryCacheStatistics,
            DataSchema = new Uri("https://bit.io/schemas/mem-cache-stats.json"),
            Data = new
            {
                result
            }
        };
    }
}