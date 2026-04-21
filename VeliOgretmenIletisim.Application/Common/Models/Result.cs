namespace VeliOgretmenIletisim.Application.Common.Models;

public class Result
{
    public bool IsSuccess { get; }
    public string Message { get; }
    public int StatusCode { get; }
    public List<string> Errors { get; } = new();

    protected Result(bool isSuccess, string message, int statusCode, List<string>? errors = null)
    {
        IsSuccess = isSuccess;
        Message = message;
        StatusCode = statusCode;
        if (errors != null) Errors = errors;
    }

    public static Result Success(string message = "Success", int statusCode = 200)
        => new(true, message, statusCode);

    public static Result Failure(string message, int statusCode = 400)
        => new(false, message, statusCode);

    public static Result Failure(List<string> errors, int statusCode = 400)
        => new(false, "Validation Error", statusCode, errors);
}

public class Result<T> : Result
{
    public T? Data { get; }

    private Result(T? data, bool isSuccess, string message, int statusCode, List<string>? errors = null)
        : base(isSuccess, message, statusCode, errors)
    {
        Data = data;
    }

    public static Result<T> Success(T data, string message = "Success", int statusCode = 200)
        => new(data, true, message, statusCode);

    public static new Result<T> Failure(string message, int statusCode = 400)
        => new(default, false, message, statusCode);

    public static new Result<T> Failure(List<string> errors, int statusCode = 400)
        => new(default, false, "Validation Error", statusCode, errors);
}
