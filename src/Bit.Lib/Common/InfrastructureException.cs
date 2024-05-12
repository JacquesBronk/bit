using Bit.Lib.Common.Base;
using Bit.Lib.Common.Exception;

namespace Bit.Lib.Common;

public abstract class InfrastructureException(string errorCode, string message, string methodName, Severity severity, string[] args, System.Exception? innerException = null)
    : BaseException(errorCode, message, methodName, severity, args, innerException);
