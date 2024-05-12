using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Bit.Log.Extensions;

public static class ServiceScopeExtensions
{
    public static void Log<TCacheManagerService>(this IServiceScope scope, LogLevel severity, FormattableString message, Exception? exception = null)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<TCacheManagerService>>();
        logger.Log(severity, exception, message.Format, message.GetArguments());
    }
}