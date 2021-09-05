using System;
using System.Linq;
using System.Threading.Tasks;
using masz.Enums;
using masz.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace masz.Middlewares
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
            } catch (BaseAPIException ex)
            {
                string message = ex.Message;
                context.Response.StatusCode = 400;
                if (ex is InvalidIdentityException)
                {
                    context.Response.StatusCode = 401;
                }
                if (ex is ResourceNotFoundException)
                {
                    context.Response.StatusCode = 404;
                }
                _logger.LogWarning($"Encountered API error type {ex.error.ToString()}, message: " + message);
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonConvert.SerializeObject(new {code = ex.error, message = message}));
            }
        }
    }
}