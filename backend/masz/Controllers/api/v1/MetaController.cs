using System;
using System.Threading.Tasks;
using masz.Enums;
using masz.Models;
using masz.Models.Views;
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
    public class MetaController : SimpleController
    {
        private readonly ILogger<MetaController> _logger;

        public MetaController(ILogger<MetaController> logger, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _logger = logger;
        }

        [HttpGet("user")]
        public IActionResult GetBotUser()
        {
            return Ok(DiscordUserView.CreateOrDefault(_discordAPI.GetCurrentBotInfo(CacheBehavior.Default)));
        }

        [HttpGet("application")]
        public IActionResult GetApplication()
        {
            return Ok(DiscordApplicationView.CreateOrDefault(_discordAPI.GetCurrentApplicationInfo()));
        }

        [HttpGet("versions")]
        public async Task<IActionResult> GetReleases() {
            var restClient = new RestClient("https://maszindex.zaanposni.com/");
            var request = new RestRequest(Method.GET);
            request.Resource = "/api/v1/versions";

            var response = await restClient.ExecuteAsync(request);

            return Ok(response.Content);
        }
    }
}