using Bit.Lib.App;
using Bit.Log.Common;

namespace Bit.Lib.Common.CloudEvent;

public static class FlagCreatedCloudEvent
{
    public static CloudNative.CloudEvents.CloudEvent ToFlagCreatedCloudEvent(this Result<(string flagName, bool IsEnabled)> result)
    {
        return new CloudNative.CloudEvents.CloudEvent
        {
            Id = Guid.NewGuid().ToString("N"),
            Source = new Uri($"https://bit.io/create/{result.Value.flagName}/{result.Value.IsEnabled}"),
            Subject = $"{EventCodes.FlagCreate}.{result.Value.flagName.ToLowerInvariant()}",
            Time = DateTime.UtcNow,
            DataSchema = new Uri("https://bit.io/schemas/flag-value.json"),
            DataContentType = DataContentTypeConstants.Json,
            Type = EventCodes.FlagCreate,
            Data = new
            {
                result.Value.flagName,
                isEnabled = result.Value.IsEnabled
            }
        };
    }
}