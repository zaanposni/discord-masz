using MASZ.Exceptions;
using Newtonsoft.Json;

namespace MASZ.Middlewares
{
    public class APIExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public APIExceptionHandlingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<APIExceptionHandlingMiddleware>();
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (BaseAPIException ex)
            {
                string message = ex.Message;
                context.Response.StatusCode = 400;
                if (ex is InvalidIdentityException || ex is UnauthorizedException)
                {
                    context.Response.StatusCode = 401;
                }
                if (ex is ResourceNotFoundException)
                {
                    context.Response.StatusCode = 404;
                }
                _logger.LogWarning($"Encountered API error type {ex.Error}, message: " + message);
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonConvert.SerializeObject(new { customMASZError = ex.Error, message }));
            }
        }
    }
}