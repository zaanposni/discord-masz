using System;
using System.Threading.Tasks;
using masz.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace masz.Controllers
{
    [ApiController]
    public class IndexController : ControllerBase
    {
        private readonly ILogger<IndexController> logger;
        private readonly IOptions<InternalConfig> config;

        public IndexController(ILogger<IndexController> logger, IOptions<InternalConfig> config)
        {
            this.logger = logger;
            this.config = config;
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
    }
}