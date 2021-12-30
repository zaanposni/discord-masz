using MASZ.Models.Views;
using MASZ.Models;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using MASZ.Repositories;

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

        [HttpGet("embed")]
        public async Task<IActionResult> GetOEmbedInfo()
        {
            AppSettings appSettings = await AppSettingsRepository.CreateDefault(_serviceProvider).GetAppSettings();

            return Ok(appSettings.GetEmbedData(_config.GetBaseUrl()));
        }

        [HttpGet("application")]
        public async Task<IActionResult> GetApplication()
        {
            return Ok(DiscordApplicationView.CreateOrDefault(await _discordAPI.GetCurrentApplicationInfo()));
        }

        [HttpGet("versions")]
        public async Task<IActionResult> GetReleases()
        {
            var restClient = new RestClient("https://maszindex.zaanposni.com/");
            var request = new RestRequest(Method.Get)
            {
                Resource = "/api/v1/versions"
            };
            request.AddQueryParameter("name", "masz_backend");

            var response = await restClient.ExecuteAsync(request);

            return Ok(response.Content);
        }
    }
}