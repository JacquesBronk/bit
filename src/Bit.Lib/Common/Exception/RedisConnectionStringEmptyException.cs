namespace Bit.Lib.Common.Exception;

public class RedisConnectionStringEmptyException(string methodName, string message, string[] args, System.Exception? innerException = null) : InfrastructureException(ExceptionCodes.Infrastructure.RedisConnectionStringEmpty, message, methodName,Severity.Critical, args, innerException);
