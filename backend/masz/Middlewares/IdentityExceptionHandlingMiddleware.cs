using System;
using System.Linq;
using System.Threading.Tasks;
using masz.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace masz.Middlewares
{
    public class IdentityExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public IdentityExceptionHandlingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<IdentityExceptionHandlingMiddleware>();
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            } catch (InvalidIdentityException)
            {
                _logger.LogWarning("Encountered invalid identity");
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized");
            } catch (UnregisteredGuildException)
            {
                _logger.LogWarning("Encountered unregistered guild");
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Unregistered guild");
            }
        }
    }
}