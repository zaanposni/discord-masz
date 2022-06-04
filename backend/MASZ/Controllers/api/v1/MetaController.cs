using MASZ.Models.Views;
using MASZ.Models;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using MASZ.Repositories;
using Discord;
using MASZ.Dtos;

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
            var restClient = new RestClient("https://maszindex.zaanposni.com/");
            var request = new RestRequest(Method.Get)
            {
                Resource = "/api/v1/versions"
            };
            request.AddQueryParameter("name", "masz_backend");

            var response = await restClient.ExecuteAsync(request);

            return Ok(response.Content);
        }

        [HttpPost("feedback")]
        public async Task<IActionResult> PostFeedback([FromBody] FeedbackDto feedback)
        {
            var restClient = new RestClient("https://maszindex.zaanposni.com/");
            var request = new RestRequest(Method.Post)
            {
                Resource = "/api/v1/feedback"
            };
            request.AddJsonBody(feedback);

            var response = await restClient.ExecuteAsync(request);

            return Ok(response.Content);
        }
    }
}