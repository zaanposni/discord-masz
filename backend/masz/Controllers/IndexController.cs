using System;
using System.Threading.Tasks;
using masz.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace masz.Controllers
{
    [ApiController]
    public class IndexController
    {
        private readonly ILogger<IndexController> logger;

        public IndexController()
        {
        }

        [HttpGet("/")]
        public IActionResult Index() {
            return new ContentResult()
            {
                Content = "<a href=\"/api/v1/login\" target=\"blank\">Login with Discord.</a>",
                ContentType = "text/html",
            };
        }
    }
}