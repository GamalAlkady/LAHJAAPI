using Microsoft.AspNetCore.Mvc;

namespace LAHJAAPI.Exceptions
{
    public class ProblemDetailsException : Exception
    {
        public ProblemDetails Problem { get; }

        public ProblemDetailsException(object obj) : base(obj is ProblemDetails problem1 ? problem1.Title : obj.ToString())
        {
            if (obj is ProblemDetails problem)
            {

                Problem = problem;
            }
            else if (obj is string str)
            {
                Problem = new ProblemDetails
                {
                    Title = str,
                    Status = 500,
                    Detail = str
                };
            }
            else if (obj is Exception ex) Problem = new ProblemDetails
            {
                Title = ex.Message,
                Status = 500,
                Detail = ex.InnerException?.Message
            };
            else if (obj is int status) Problem = new ProblemDetails
            {
                Title = "Error",
                Status = status,
                Detail = "An error occurred"
            };
            else Problem = new ProblemDetails
            {
                Title = nameof(obj),
                Detail = "Invalid object type",
                Status = 500,
                Type = typeof(ProblemDetailsException).FullName
            };
        }
    }

}
