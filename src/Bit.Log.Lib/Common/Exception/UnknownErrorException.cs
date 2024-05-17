namespace Bit.Log.Common.Exception;

public class UnknownErrorException(string message, string methodName, string[] args, System.Exception? innerException = null) : DomainException("UNKNOWN_DOMAIN_ERROR", message, methodName, Severity.Critical, args, innerException);
