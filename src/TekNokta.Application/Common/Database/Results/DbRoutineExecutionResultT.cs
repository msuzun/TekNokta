namespace TekNokta.Application.Common.Database.Results;

public sealed class DbRoutineExecutionResult<T>
{
    private DbRoutineExecutionResult(
        bool isSuccess,
        string? message,
        string? errorCode,
        T? data)
    {
        IsSuccess = isSuccess;
        Message = message;
        ErrorCode = errorCode;
        Data = data;
    }

    public bool IsSuccess { get; }

    public string? Message { get; }

    public string? ErrorCode { get; }

    public T? Data { get; }

    public static DbRoutineExecutionResult<T> Success(T data, string? message = null)
    {
        return new DbRoutineExecutionResult<T>(true, message, null, data);
    }

    public static DbRoutineExecutionResult<T> Failure(string message, string? errorCode = null)
    {
        return new DbRoutineExecutionResult<T>(false, message, errorCode, default);
    }
}
