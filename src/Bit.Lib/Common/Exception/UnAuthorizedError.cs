namespace Bit.Lib.Common.Exception;

public class UnAuthorizedError(string methodName, string message, string[] args, System.Exception? innerException = null) : InfrastructureException(ExceptionCodes.Infrastructure.Unauthorized, message, methodName,Severity.Critical, args, innerException);
