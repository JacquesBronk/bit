namespace Bit.Lib.Common.Exception;
public class CannotFindKeyException(string methodName, string message, string[] args, System.Exception? innerException = null) : InfrastructureException(ExceptionCodes.Data.CannotFindKey, message, methodName,Severity.Info, args, innerException);
