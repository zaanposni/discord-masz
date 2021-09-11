using System;
using System.Linq;
using System.Threading.Tasks;
using masz.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace masz.Middlewares
{
    public class HeaderMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly IOptions<InternalConfig> _config;

        public HeaderMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IOptions<InternalConfig> config)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<HeaderMiddleware>();
            _config = config;
        }

        public async Task Invoke(HttpContext context)
        {
            _logger.LogInformation("Setting header host " + _config.Value.ServiceHostName);
            context.Request.Headers["Host"] = _config.Value.ServiceHostName;
            await _next(context);
        }
    }
}