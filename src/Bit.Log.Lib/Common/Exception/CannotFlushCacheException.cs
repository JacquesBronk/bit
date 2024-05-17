namespace Bit.Log.Common.Exception;
public class CannotFlushCacheException(string methodName, string message, string[] args, System.Exception? innerException = null) : InfrastructureException(ExceptionCodes.Infrastructure.CannotFlushCache, message, methodName,Severity.Error, args, innerException);
