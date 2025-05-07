using LAHJAAPI.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace LAHJAAPI.Middlewares
{
    public class ProblemDetailsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ProblemDetailsMiddleware> _logger;

        public ProblemDetailsMiddleware(RequestDelegate next, ILogger<ProblemDetailsMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ProblemDetailsException ex)
            {
                context.Response.StatusCode = ex.Problem.Status ?? 500;
                context.Response.ContentType = "application/problem+json";
                await context.Response.WriteAsJsonAsync(ex.Problem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");

                var problem = new ProblemDetails
                {
                    Title = ex.Message,
                    Status = 500,
                    Detail = ex.InnerException?.Message,
                    Instance = context.Request.Path
                };

                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/problem+json";
                await context.Response.WriteAsJsonAsync(problem);
            }
        }
    }

}
