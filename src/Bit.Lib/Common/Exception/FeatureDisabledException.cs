namespace Bit.Lib.Common.Exception;

public class FeatureDisabledException(string methodName, string[] args, System.Exception? innerException = null) : InfrastructureException(EventCodes.FlagIsDisabled, "This feature is currently disabled", methodName,Severity.Info, args, innerException);
 