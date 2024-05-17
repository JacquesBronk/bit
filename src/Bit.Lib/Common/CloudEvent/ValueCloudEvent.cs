using Bit.Lib.App;
using Bit.Log.Common;

namespace Bit.Lib.Common.CloudEvent;

public static class ValueCloudEvent
{
    public static CloudNative.CloudEvents.CloudEvent ToHasValueEvent(this Result<(string flagName, bool IsEnabled)> result)
    {
        if (result.IsFailure)
        {
            return new CloudNative.CloudEvents.CloudEvent
            {
                Id = Guid.NewGuid().ToString("N"),
                Source = new Uri($"https://bit.io/{result.Value.flagName}"),
                Subject = $"{EventCodes.FlagFetch}.{result.Value.flagName}",
                Time = DateTime.UtcNow,
                DataSchema = new Uri("https://bit.io/schemas/flag-value.json"),
                DataContentType = DataContentTypeConstants.Json,
                Type = ExceptionCodes.Data.CannotFindKey,
                Data = new
                {
                    result.Value.flagName
                }
            };
        }
        return new CloudNative.CloudEvents.CloudEvent
        {
            Id = Guid.NewGuid().ToString("N"),
            Subject = $"{EventCodes.FlagFetch}.{result.Value.flagName}",
            Time = DateTime.UtcNow,
            Source = new Uri($"https://bit.io/{result.Value.flagName}"),
            DataContentType = DataContentTypeConstants.Json,
            Type = EventCodes.FlagFetch,
            DataSchema = new Uri("https://bit.io/schemas/flag-value.json"),
            Data = new
            {
                result.Value.flagName,
                isEnabled = result.Value.IsEnabled
            }
        };
    }
}