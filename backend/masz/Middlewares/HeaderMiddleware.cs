using System;
using System.Linq;
using System.Threading.Tasks;
using masz.Models;
using masz.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace masz.Middlewares
{
    public class HeaderMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly IInternalConfiguration _config;

        public HeaderMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IInternalConfiguration config)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<HeaderMiddleware>();
            _config = config;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Request.Headers["Host"] = _config.GetServiceDomain();
            await _next(context);
        }
    }
}