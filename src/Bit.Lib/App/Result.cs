using System.Diagnostics.CodeAnalysis;
using Bit.Log.Common;
using Bit.Log.Common.Exception;

namespace Bit.Lib.App;

public class Result<T>
{
    public T? Value { get; private init; } 
    public bool IsSuccess { get; init; }
    public Exception Error { get; init; } = new IgnoreException();

    private string ErrorCode { get; init; } = string.Empty;
    public bool IsFailure => !IsSuccess;

    public bool HasValue => Value != null;

    private Result() { }

    public T? GetValueOrThrow()
    {
        if (IsSuccess)
            return Value;
        else
            throw Error;
    }

    public T? GetValueOrDefault(T? defaultValue)
    {
        if (IsSuccess)
            return Value;
        else
            return defaultValue;
    }

    public T? GetValueOrDefault(Func<T> defaultValueFactory)
    {
        if (IsSuccess)
            return Value;
        else
            return defaultValueFactory();
    }

    [SuppressMessage("ReSharper", "NullableWarningSuppressionIsUsed")]
    public Result<T> OnSuccess(Action<T> action)
    {
        if (IsSuccess && HasValue)
        {
            action(Value!);
        }
            

        return this;
    }

    public Result<T> OnFailure(Action<Exception> action)
    {
        if (IsFailure)
            action(Error);

        return this;
    }

    public static implicit operator T?(Result<T?> result) => result.GetValueOrThrow();

    public static Result<T> Success(T value) => new Result<T> { Value = value, IsSuccess = true };

    public static Result<T> Failure(Exception error)
    {
        if (error == null)
            throw new ArgumentNullException(nameof(error));

        string errorCode = error switch
        {
            InfrastructureException infraEx => infraEx.ErrorCode,
            AppException appEx => appEx.ErrorCode,
            DomainException domainEx => domainEx.ErrorCode,
            _ => "UnknownError"
        };

        return new Result<T> { Error = error, ErrorCode = errorCode, IsSuccess = false };
    }

    public override string ToString()
    {
        if (IsSuccess)
            return $"Success: {Value}";
        else
            return $"Failure: {Error.Message}, ErrorCode: {ErrorCode} , StackTrace: {Error.StackTrace}, InnerException: {Error.InnerException}, Data: {Error.Data}";
    }
}