namespace Bit.Lib.Common.Exception;

public class FromCloudEventException(string methodName, string[] args, System.Exception? innerException = null) : InfrastructureException("infra.serialization.deserialize.cloud-event", "Cannot deserialize the cloud event", methodName,Severity.Error, args, innerException);
