using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace masz.Controllers
{
    [ApiController]
    [Route("api/v1/")]
    public class StatusController : SimpleController
    {
        private readonly ILogger<StatusController> _logger;

        public StatusController(ILogger<StatusController> logger, IServiceProvider serviceProvider): base(serviceProvider)
        {
            _logger = logger;
        }

        [HttpGet("status")]
        [HttpGet("health")]
        [HttpGet("healthcheck")]
        [HttpGet("ping")]
        public IActionResult Status() {
            var accept = String.Empty;
            if (HttpContext.Request.Headers.ContainsKey("Accept")) {
                if (HttpContext.Request.Headers["Accept"].ToString().Contains("application/json"))
                {
                    return Ok(new {
                        status = "ok",
                        lang = _config.GetDefaultLanguage(),
                        name = _config.GetHostName(),
                        server_time = DateTime.Now.ToString(),
                        server_time_utc = DateTime.UtcNow.ToString()
                    });
                }
            }
            return Ok("OK");
        }
    }
}