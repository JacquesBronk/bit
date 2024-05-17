using Bit.Lib.App;
using Bit.Log.Common;

namespace Bit.Lib.Common.CloudEvent;

public static class FlagDeletedCloudEvent
{
    public static CloudNative.CloudEvents.CloudEvent ToFlagDeletedCloudEvent(this Result<(string flagName, bool IsEnabled)> result)
    {
        return new CloudNative.CloudEvents.CloudEvent
        {
            Id = Guid.NewGuid().ToString("N"),
            Source = new Uri($"https://bit.io/delete/{result.Value.flagName}"),
            Subject = $"{EventCodes.FlagDelete}.{result.Value.flagName.ToLowerInvariant()}",
            Time = DateTime.UtcNow,
            DataSchema = new Uri("https://bit.io/schemas/flag-value.json"),
            DataContentType = DataContentTypeConstants.Json,
            Type = EventCodes.FlagDelete,
            Data = new
            {
                result.Value.flagName,
                isEnabled = result.Value.IsEnabled
            }
        };
    }
}