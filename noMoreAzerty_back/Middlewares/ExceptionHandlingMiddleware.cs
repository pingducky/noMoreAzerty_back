using noMoreAzerty_back.Exceptions;

namespace noMoreAzerty_back.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger,
            IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleAsync(context, ex);
            }
        }

        private async Task HandleAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            var statusCode = ex is ApiException apiEx
                ? apiEx.StatusCode
                : StatusCodes.Status500InternalServerError;

            if (statusCode >= 500)
                _logger.LogError(ex, ex.Message);
            else
                _logger.LogWarning(ex, ex.Message);

            var response = new
            {
                status = statusCode,
                message = _env.IsDevelopment()
                    ? ex.Message
                    : "Une erreur est survenue",
                traceId = context.TraceIdentifier
            };

            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
