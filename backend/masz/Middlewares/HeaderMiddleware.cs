using MASZ.Services;

namespace MASZ.Middlewares
{
    public class HeaderMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly InternalConfiguration _config;

        public HeaderMiddleware(RequestDelegate next, InternalConfiguration config)
        {
            _next = next;
            _config = config;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Request.Headers["Host"] = _config.GetServiceDomain();
            await _next(context);
        }
    }
}