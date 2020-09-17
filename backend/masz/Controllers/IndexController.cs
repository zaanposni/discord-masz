using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace masz.Controllers
{
    [ApiController]
    public class IndexController : ControllerBase
    {
        private readonly ILogger<IndexController> logger;

        public IndexController(ILogger<IndexController> logger)
        {
            this.logger = logger;
        }

        [HttpGet("/")]
        public IActionResult Index() {
            logger.LogInformation("/ | Returning Index.");
            return new ContentResult() 
            {
                Content = "<a href=\"/api/v1/login\" target=\"blank\">Login with Discord.</a>",
                ContentType = "text/html",
            };
        }

        [HttpGet("/api/v1/status")]
        [HttpGet("/api/v1/health")]
        [HttpGet("/api/v1/healthcheck")]
        [HttpGet("/api/v1/ping")]
        public IActionResult Status() {
            logger.LogInformation("/status | Returning Status.");
            return Ok("OK");
        }
    }
}