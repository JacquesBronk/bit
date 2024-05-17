namespace Bit.Log.Common.Exception;

public enum Severity
{
    Critical,
    Error,
    Warning,
    Info,
    Debug
}

public static class Overrides
{
    public static string ToString(this Severity severity)
    {
        return severity switch
        {
            Severity.Critical => "Critical",
            Severity.Error => "Error",
            Severity.Warning => "Warning",
            Severity.Info => "Info",
            Severity.Debug => "Debug",
            _ => throw new ArgumentOutOfRangeException(nameof(severity), severity, null)
        };
    }
}