﻿namespace Bit.Log.Common.Exception;
public class CannotDeleteFlagException(string methodName, string message, string[] args, System.Exception? innerException = null) : InfrastructureException(ExceptionCodes.Data.CannotDeleteFlag, message, methodName, Severity.Error, args, innerException);