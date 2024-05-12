using Bit.Lib.App;

namespace Bit.Lib.Common.CloudEvent;

public static class CacheFlushedCloudEvent
{
    public static CloudNative.CloudEvents.CloudEvent ToCacheFlushedCloudEvent(this Result<bool> result)
    {
        if (result.IsFailure)
        {
            return new CloudNative.CloudEvents.CloudEvent
            {
                Id = Guid.NewGuid().ToString("N"),
                Source = new Uri($"https://bit.io/flush"),
                Subject = $"{ExceptionCodes.Infrastructure.CannotFlushCache}",
                Time = DateTime.UtcNow,
                DataSchema = new Uri("https://bit.io/schemas/flag-cache-flushed.json"),
                DataContentType = DataContentTypeConstants.Json,
                Type = ExceptionCodes.Infrastructure.CannotFlushCache,
                Data = new
                {
                   Error = result.ToString()
                }
            };
        }
        return new CloudNative.CloudEvents.CloudEvent
        {
            Id = Guid.NewGuid().ToString("N"),
            Subject = $"{EventCodes.CacheFlushed}",
            Time = DateTime.UtcNow,
            Source = new Uri($"https://bit.io/flush"),
            DataContentType = DataContentTypeConstants.Json,
            Type = EventCodes.CacheFlushed,
            DataSchema = new Uri("https://bit.io/schemas/flag-cache-flushed.json"),
            Data = new
            {
                result
            }
        };
       
    }
}