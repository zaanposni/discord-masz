using System;
using System.Threading.Tasks;
using masz.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestSharp;

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
        public IActionResult GetClientId() {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");

            return Ok(new { clientid = config.Value.DiscordClientId });
        }

        [HttpGet("githubreleases")]
        public async Task<IActionResult> GetReleases() {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");

            var restClient = new RestClient("https://api.github.com/");
            var request = new RestRequest(Method.GET);
            request.Resource = "/repos/zaanposni/discord-masz/releases";

            var response = await restClient.ExecuteAsync(request);

            return Ok(response.Content);
        }
    }
}