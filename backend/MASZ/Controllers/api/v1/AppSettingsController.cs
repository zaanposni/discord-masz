using Discord;
using MASZ.Dtos.AppSettings;
using MASZ.Dtos.GuildMotd;
using MASZ.Enums;
using MASZ.Models;
using MASZ.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MASZ.Controllers
{
    [ApiController]
    [Route("api/v1/settings")]
    [Authorize]
    public class AppSettingsController : SimpleController
    {

        public AppSettingsController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetAppSettings([FromRoute] ulong guildId)
        {
            await RequireSiteAdmin();

            return Ok(await AppSettingsRepository.CreateDefault(_serviceProvider).GetAppSettings());
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAppSettings([FromBody] AppSettingsForPutDto newSettings)
        {
            await RequireSiteAdmin();

            AppSettings toAdd = new AppSettings()
            {
                EmbedTitle = newSettings.EmbedTitle,
                EmbedContent = newSettings.EmbedContent
            };

            return Ok(await AppSettingsRepository.CreateDefault(_serviceProvider).UpdateAppSettings(toAdd));
        }
    }
}