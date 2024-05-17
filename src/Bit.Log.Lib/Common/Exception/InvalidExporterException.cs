namespace Bit.Log.Common.Exception;

public class InvalidExporterException(string methodName, string message, string[] args, System.Exception? innerException = null) : InfrastructureException(ExceptionCodes.Infrastructure.InvalidOtlpExporter, message, methodName,Severity.Critical, args, innerException);
