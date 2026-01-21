using Microsoft.AspNetCore.Mvc;
using noMoreAzerty_back.Exceptions;

namespace noMoreAzerty_back.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IHostEnvironment _environment;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger,
            IHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/problem+json";

            var (problemDetails, logLevel) = exception switch
            {
                ValidationException validationEx => (CreateValidationProblemDetails(
                    detail: "One or more validation errors occurred",
                    instance: context.Request.Path,
                    errors: validationEx.Errors
                ), LogLevel.Warning),

                ApiException apiEx => (CreateProblemDetails(
                    title: GetTitleForException(apiEx),
                    detail: apiEx.Message,
                    status: apiEx.StatusCode,
                    instance: context.Request.Path
                ), apiEx.StatusCode >= 500 ? LogLevel.Error : LogLevel.Warning),

                UnauthorizedAccessException => (CreateProblemDetails(
                    title: "Unauthorized",
                    detail: "Authentication required",
                    status: StatusCodes.Status401Unauthorized,
                    instance: context.Request.Path
                ), LogLevel.Warning),

                _ => (CreateProblemDetails(
                    title: "Internal Server Error",
                    detail: _environment.IsDevelopment()
                        ? exception.Message
                        : "An unexpected error occurred. Please try again later.",
                    status: StatusCodes.Status500InternalServerError,
                    instance: context.Request.Path
                ), LogLevel.Error)
            };

            context.Response.StatusCode = problemDetails.Status ?? 500;

            _logger.Log(logLevel, exception,
                "Request {Method} {Path} failed with status {StatusCode}: {Message}",
                context.Request.Method,
                context.Request.Path,
                problemDetails.Status,
                exception.Message);

            await context.Response.WriteAsJsonAsync(problemDetails);
        }

        private static ProblemDetails CreateProblemDetails(
            string title,
            string detail,
            int status,
            string instance)
        {
            return new ProblemDetails
            {
                Type = $"https://httpstatuses.com/{status}",  // URI qui documente le type d'erreur
                Title = title,
                Detail = detail,
                Status = status,
                Instance = instance
            };
        }

        private static ValidationProblemDetails CreateValidationProblemDetails(
            string detail,
            string instance,
            Dictionary<string, string[]> errors)
        {
            return new ValidationProblemDetails(errors)
            {
                Type = "https://httpstatuses.com/400",
                Title = "Validation Error",
                Detail = detail,
                Status = StatusCodes.Status400BadRequest,
                Instance = instance
            };
        }

        private static string GetTitleForException(ApiException exception)
        {
            return exception switch
            {
                NotFoundException => "Resource Not Found",
                ForbiddenException => "Forbidden",
                _ => "API Error"
            };
        }
    }
}
