using MASZ.Models.Views;
using Microsoft.AspNetCore.Mvc;
using RestSharp;

namespace MASZ.Controllers
{
    [ApiController]
    [Route("api/v1/meta/")]
    public class MetaController : SimpleController
    {
        public MetaController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpGet("user")]
        public IActionResult GetBotUser()
        {
            return Ok(DiscordUserView.CreateOrDefault(_discordAPI.GetCurrentBotInfo()));
        }

        [HttpGet("application")]
        public async Task<IActionResult> GetApplication()
        {
            return Ok(DiscordApplicationView.CreateOrDefault(await _discordAPI.GetCurrentApplicationInfo()));
        }

        [HttpGet("versions")]
        public async Task<IActionResult> GetReleases()
        {
            var restClient = new RestClient("https://MASZindex.zaanposni.com/");
            var request = new RestRequest(Method.GET)
            {
                Resource = "/api/v1/versions"
            };

            var response = await restClient.ExecuteAsync(request);

            return Ok(response.Content);
        }
    }
}