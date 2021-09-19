using System;
using System.Threading.Tasks;
using masz.Models;
using masz.Services;
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
        private readonly ILogger<MetaController> _logger;
        private readonly IInternalConfiguration _config;

        public MetaController(ILogger<MetaController> logger, IInternalConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        [HttpGet("clientid")]
        public IActionResult GetClientId() {
            return Ok(new { clientid = _config.GetClientId() });
        }

        [HttpGet("githubreleases")]
        public async Task<IActionResult> GetReleases() {
            var restClient = new RestClient("https://api.github.com/");
            var request = new RestRequest(Method.GET);
            request.Resource = "/repos/zaanposni/discord-masz/releases";

            var response = await restClient.ExecuteAsync(request);

            return Ok(response.Content);
        }
    }
}