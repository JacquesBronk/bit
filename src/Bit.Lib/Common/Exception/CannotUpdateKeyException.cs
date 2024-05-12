﻿namespace Bit.Lib.Common.Exception;
public class CannotUpdateKeyException(string methodName, string message, string[] args, System.Exception? innerException = null) : InfrastructureException(ExceptionCodes.Data.CannotUpdateKey, message, methodName, Severity.Error, args, innerException);