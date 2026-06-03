namespace TekNokta.Application.Common.Database.Results;

public class DbRoutineExecutionResult
{
    protected DbRoutineExecutionResult(
        bool isSuccess,
        string? message,
        string? errorCode,
        int? affectedRows)
    {
        IsSuccess = isSuccess;
        Message = message;
        ErrorCode = errorCode;
        AffectedRows = affectedRows;
    }

    public bool IsSuccess { get; }

    public string? Message { get; }

    public string? ErrorCode { get; }

    public int? AffectedRows { get; }

    public static DbRoutineExecutionResult Success(string? message = null, int? affectedRows = null)
    {
        return new DbRoutineExecutionResult(true, message, null, affectedRows);
    }

    public static DbRoutineExecutionResult Failure(string message, string? errorCode = null)
    {
        return new DbRoutineExecutionResult(false, message, errorCode, null);
    }
}
