namespace Bit.Log.Common.Exception;
public class CannotCreateFlagException(string methodName, string message, string[] args, System.Exception? innerException = null) : InfrastructureException(ExceptionCodes.Data.CannotCreateFlag, message, methodName,Severity.Error, args, innerException);
