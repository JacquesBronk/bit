using Bit.Log.Common.Base;
using Bit.Log.Common.Exception;

namespace Bit.Log.Common;

public abstract class DomainException(string errorCode, string message, string methodName, Severity severity, string[] args, System.Exception? innerException = null)
    : BaseException(errorCode, message, methodName, severity, args, innerException);