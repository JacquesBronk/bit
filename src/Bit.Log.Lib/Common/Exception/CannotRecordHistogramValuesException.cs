namespace Bit.Log.Common.Exception;

public class CannotRecordHistogramValuesException(string methodName, string message, string[] args, System.Exception? innerException = null)
    : InfrastructureException(ExceptionCodes.Data.CannotRecordHistogram, message, methodName, Severity.Error, args, innerException);