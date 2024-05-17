namespace Bit.Log.Common.Exception;

public class CannotGetKeyCountException(string methodName, string message, string[] args, System.Exception? innerException = null) : InfrastructureException(ExceptionCodes.Infrastructure.RedisConnectionStringEmpty, message, methodName,Severity.Error, args, innerException);
