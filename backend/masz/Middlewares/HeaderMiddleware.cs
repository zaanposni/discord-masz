using MASZ.Services;

namespace MASZ.Middlewares
{
    public class HeaderMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IInternalConfiguration _config;

        public HeaderMiddleware(RequestDelegate next, IInternalConfiguration config)
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