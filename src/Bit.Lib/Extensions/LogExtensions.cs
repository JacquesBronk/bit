using Bit.Lib.Common;
using Bit.Log.Common;
using Bit.Log.Common.Exception;
using Microsoft.Extensions.Logging;

namespace Bit.Lib.Extensions;

public static class LogExtensions
{
    public static void LogError(this ILogger logger, Exception ex)
    {
        switch (ex)
        {
            case InfrastructureException infraEx:
                logger.LogExceptionWithSeverity(ex, infraEx.Severity);
                break;
            case AppException applicationException:
                logger.LogExceptionWithSeverity(ex, applicationException.Severity);
                break;
            case DomainException domainException:
                logger.LogExceptionWithSeverity(ex, domainException.Severity);
                break;
            default:
                logger.LogExceptionWithSeverity(ex, Severity.Error);
                break;
        }
    }

    private static void LogExceptionWithSeverity(this ILogger logger, Exception ex, Severity severity)
    {
        switch (severity)
        {
            case Severity.Info:
                logger.LogInformation("An exception with Info severity occurred: {Exception}", ex);
                break;
            case Severity.Warning:
                logger.LogWarning("An exception with Warning severity occurred: {Exception}", ex);
                break;
            case Severity.Error:
                logger.LogError("An exception with Error severity occurred: {Exception}", ex);
                break;
            case Severity.Critical:
                logger.LogCritical("An exception with Critical severity occurred: {Exception}", ex);
                break;
            default:
                logger.LogTrace("An exception with Trace severity occurred: {Exception}", ex);
                break;
        }
    }
}