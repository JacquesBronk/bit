using Bit.Log.Common;

namespace Bit.Lib.Common.CloudEvent;

public static class FlagIsDisabledCloudEvent
{
    public static CloudNative.CloudEvents.CloudEvent ToIsDisabledEvent(this (string flagName,  bool IsEnabled) result)
    {
        return new CloudNative.CloudEvents.CloudEvent()
        {
            Id = Guid.NewGuid().ToString("N"),
            Subject = $"{EventCodes.FlagIsDisabled}.{result.flagName}",
            Time = DateTime.UtcNow,
            Source = new Uri($"https://bit.io/isEnabled/{result.flagName}"),
            DataContentType = DataContentTypeConstants.Json,
            Type = EventCodes.FlagIsDisabled,
            DataSchema = new Uri("https://bit.io/schemas/flag-value.json"),
            Data = new
            {
                result.flagName,
                isEnabled = result.IsEnabled
            }
        };
    }
}