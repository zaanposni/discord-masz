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
        public async Task<IActionResult> Index() {
            logger.LogInformation("/ | Returning Index.");
            return new ContentResult() 
            {
                Content = "<a href=\"/api/v1/login\" target=\"blank\">Login with Discord.</a>",
                ContentType = "text/html",
            };
        }

        [HttpGet("/api/v1/status")]
        public async Task<IActionResult> Status() {
            logger.LogInformation("/status | Returning Status.");
            string headers = String.Empty;
            foreach (var key in HttpContext.Request.Headers.Keys)
                headers += key + "=" + HttpContext.Request.Headers[key] + Environment.NewLine;
            logger.LogInformation(headers);
            return Ok("OK");
        }
    }
}