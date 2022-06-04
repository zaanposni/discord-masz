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

            AppSettingsRepository repo = AppSettingsRepository.CreateDefault(_serviceProvider);

            AppSettings current = await repo.GetAppSettings();

            current.EmbedTitle = newSettings.EmbedTitle;
            current.EmbedContent = newSettings.EmbedContent;
            current.EmbedShowIcon = newSettings.EmbedShowIcon;
            current.DefaultLanguage = newSettings.DefaultLanguage;
            current.AuditLogWebhookURL = newSettings.AuditLogWebhookURL ?? string.Empty;
            current.PublicFileMode = newSettings.PublicFileMode;

            return Ok(await repo.UpdateAppSettings(current));
        }
    }
}