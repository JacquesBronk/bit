namespace Bit.Log.Common.Exception;
public class ApiKeyNotFoundException(string methodName, string[] args, System.Exception? innerException = null) : InfrastructureException(ExceptionCodes.Infrastructure.ApiKeyNotFound, "Api key configuration missing", methodName,Severity.Critical, args, innerException);
