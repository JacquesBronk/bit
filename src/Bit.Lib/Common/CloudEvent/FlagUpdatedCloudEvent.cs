using Bit.Lib.App;

namespace Bit.Lib.Common.CloudEvent;

public static class FlagUpdatedCloudEvent
{
    public static CloudNative.CloudEvents.CloudEvent ToFlagUpdatedCloudEvent(this Result<(string flagName, bool IsEnabled)> result)
    {
        return new CloudNative.CloudEvents.CloudEvent
        {
            Id = Guid.NewGuid().ToString("N"),
            Subject = $"{EventCodes.FlagUpdate}.{result.Value.flagName.ToLowerInvariant()}",
            Source = new Uri($"https://bit.io/update/{result.Value.flagName}/{result.Value.IsEnabled}"),
            Time = DateTime.UtcNow,
            DataSchema = new Uri("https://bit.io/schemas/flag-value.json"),
            DataContentType = DataContentTypeConstants.Json,
            Type = EventCodes.FlagUpdate,
            Data = new
            {
                result.Value.flagName,
                isEnabled = result.Value.IsEnabled
            }
        };
    }
}