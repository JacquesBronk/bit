namespace Bit.Lib.Common.CloudEvent;

public static class FlagIsEnabledCloudEvent
{
    public static CloudNative.CloudEvents.CloudEvent ToIsEnabledEvent(this (string flagName, bool IsEnabled) result)
    {
        return new CloudNative.CloudEvents.CloudEvent()
        {
            Id = Guid.NewGuid().ToString("N"),
            Subject = $"{EventCodes.FlagIsEnabled}.{result.flagName}",
            Time = DateTime.UtcNow,
            Source = new Uri($"https://bit.io/isEnabled/{result.flagName}"),
            DataContentType = DataContentTypeConstants.Json,
            Type = EventCodes.FlagIsEnabled,
            DataSchema = new Uri("https://bit.io/schemas/flag-value.json"),
            Data = new
            {
                result.flagName,
                isEnabled = result.IsEnabled
            }
        };
    }
}