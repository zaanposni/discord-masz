using System;
using masz.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace masz.Controllers
{
    [ApiController]
    [Route("api/v1/meta/")]
    [Authorize]
    public class MetaController : ControllerBase
    {
        private readonly ILogger<MetaController> logger;
        private readonly IOptions<InternalConfig> config;

        public MetaController(ILogger<MetaController> logger, IOptions<InternalConfig> config)
        {
            this.logger = logger;
            this.config = config;
        }

        [HttpGet("clientid")]
        public IActionResult Status() {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");

            return Ok(new { clientid = config.Value.DiscordClientId });
        }
    }
}