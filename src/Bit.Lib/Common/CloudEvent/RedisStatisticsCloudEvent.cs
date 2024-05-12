using Bit.Lib.App;

namespace Bit.Lib.Common.CloudEvent;

public static class RedisStatisticsCloudEvent
{
    public static CloudNative.CloudEvents.CloudEvent ToRedisStatsEvent(this Result<int>? result)
    {
        if (result is null)
        {
            return new CloudNative.CloudEvents.CloudEvent
            {
                Id = Guid.NewGuid().ToString("N"),
                Source = new Uri($"https://bit.io/redis-stats"),
                Subject = $"{EventCodes.FlagRedisCacheStatistics}",
                Time = DateTime.UtcNow,
                DataSchema = new Uri("https://bit.io/schemas/redis-stats.json"),
                DataContentType = DataContentTypeConstants.Json,
                Type = ExceptionCodes.Infrastructure.CannotGetRedisCacheStatistics,
                Data = new
                {
                    KeyCount = result?.Value ?? 0
                }
            };
        }
        return new CloudNative.CloudEvents.CloudEvent
        {
            Id = Guid.NewGuid().ToString("N"),
            Subject = $"{EventCodes.FlagMemoryCacheStatistics}",
            Time = DateTime.UtcNow,
            Source = new Uri($"https://bit.io/redis-stats"),
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