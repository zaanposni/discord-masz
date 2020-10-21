using System;
using masz.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace masz.Controllers
{
    [ApiController]
    [Route("api/v1/")]
    public class StatusController : ControllerBase
    {
        private readonly ILogger<StatusController> logger;
        private readonly IOptions<InternalConfig> config;

        public StatusController(ILogger<StatusController> logger, IOptions<InternalConfig> config)
        {
            this.logger = logger;
            this.config = config;
        }

        [HttpGet("status")]
        [HttpGet("health")]
        [HttpGet("healthcheck")]
        [HttpGet("ping")]
        public IActionResult Status() {
            logger.LogInformation("/status | Returning Status.");

            var accept = String.Empty;
            if (HttpContext.Request.Headers.ContainsKey("Accept")) {
                if (HttpContext.Request.Headers["Accept"].ToString().Contains("application/json"))
                {
                    return Ok(new {
                        status = "ok",
                        name = config.Value.ServiceHostName,
                        server_time = DateTime.Now.ToString(),
                        server_time_utc = DateTime.UtcNow.ToString()
                    });
                }
            }
            return Ok("OK");
        }
    }
}