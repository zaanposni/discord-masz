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
    }
}