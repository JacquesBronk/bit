using System.Text.Json;
using CloudNative.CloudEvents;
using CloudNative.CloudEvents.SystemTextJson;

namespace Bit.Lib.Infra;

public static class CloudEventJson
{
    public static string ConvertCloudEventToJson(this CloudEvent cloudEvent)
    {
        var formatter = new JsonEventFormatter();
        var cloudEventJson = formatter.EncodeStructuredModeMessage(cloudEvent, out var contentType);
        return System.Text.Encoding.UTF8.GetString(cloudEventJson.ToArray());
    }
}