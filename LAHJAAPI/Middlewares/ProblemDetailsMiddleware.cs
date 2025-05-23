using AutoGenerator.Repositories.Base;
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
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Unhandled RepositoryException");

                var problem = new ProblemDetails
                {
                    Title = ex.Message,
                    Status = 500,
                    Type = ex.Source,
                    Detail = ex.InnerException?.InnerException?.Message,
                    Instance = context.Request.Path
                };

                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/problem+json";
                await context.Response.WriteAsJsonAsync(problem);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled Exception");

                var problem = new ProblemDetails
                {
                    Title = ex.Message,
                    Status = 500,
                    Detail = ex.InnerException?.Message,
                    Type = ex.Source,
                    Instance = context.Request.Path
                };

                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/problem+json";
                await context.Response.WriteAsJsonAsync(problem);
            }
        }
    }

}
