using Microsoft.AspNetCore.Mvc;
using WasmAI.ConditionChecker.Base;


public class HandleResult
{
    private readonly object _value;

    public bool IsSuccess { get; }
    public bool IsFailuer => !IsSuccess;

    public object Data
    {
        get
        {
            //if (!IsSuccess) throw new InvalidOperationException("there is no value for failure");
            return _value;
        }
        private init => _value = value;
    }


    public HandleResult(object value)
    {
        Data = value;
        IsSuccess = true;
        //Error = Error.None;
    }

    public HandleResult(bool IsSuccess = true) { this.IsSuccess = IsSuccess; }



    public static HandleResult Ok()
    {
        return new HandleResult();
    }

    public static HandleResult Ok(object value)
    {
        var r = new HandleResult(value);
        return r;
    }

    public static string Text(string str)
    {
        return str;
    }

    public static ProblemDetails Error(string str)
    {
        return Problem("An error occurred.", str);
    }

    public static ProblemDetails Problem(ProblemDetails problem)
    {
        return problem;
    }


    public static ProblemDetails Problem(ConditionResult result)
    {
        if (result.Result is ProblemDetails problem)
            return problem;
        return new ProblemDetails
        {
            Title = "An error occurred",
            Detail = result.Message
        };
    }

    public static ProblemDetails Problem(Exception ex)
    {
        return new ProblemDetails
        {
            Type = ex.GetType().FullName,
            Title = ex.Message,
            Detail = ex.InnerException?.Message,

            //Status=ex.cod
        };
    }

    public static ProblemDetails Problem(string title, string details, string? type = null, int? status = null)
    {
        return new ProblemDetails
        {
            Title = title,
            Detail = details,
            Type = type,
            Status = status
        };
    }

    public static ProblemDetails NotFound(string details, string? type = null, string title = "Not Found", int status = StatusCodes.Status404NotFound)
    {
        return new ProblemDetails
        {
            Title = title,
            Detail = details,
            Type = type,
            Status = status
        };
    }
}
