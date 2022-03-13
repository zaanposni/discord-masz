using Discord;
using MASZ.Dtos.GuildConfig;
using MASZ.Enums;
using MASZ.Exceptions;
using MASZ.Models;
using MASZ.Models.Views;
using MASZ.Repositories;
using MASZ.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MASZ.Controllers
{
    [ApiController]
    [Route("api/v1/guilds/")]
    [Authorize]
    public class GuildConfigController : SimpleController
    {
        public GuildConfigController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpGet("{guildId}")]
        public async Task<IActionResult> GetSpecificItem([FromRoute] ulong guildId)
        {
            await RequirePermission(guildId, DiscordPermission.Admin);

            return Ok(new GuildConfigView(await GuildConfigRepository.CreateDefault(_serviceProvider).GetGuildConfig(guildId)));
        }

        [HttpDelete("{guildId}")]
        public async Task<IActionResult> DeleteSpecificItem([FromRoute] ulong guildId, [FromQuery] bool deleteData = false)
        {
            await RequireSiteAdmin();
            await GetRegisteredGuild(guildId);

            await GuildConfigRepository.CreateDefault(_serviceProvider).DeleteGuildConfig(guildId, deleteData);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> CreateItem([FromBody] GuildConfigForCreateDto guildConfigForCreateDto, [FromQuery] bool importExistingBans = false)
        {
            await RequireSiteAdmin();

            try
            {
                GuildConfig alreadyRegistered = await GetRegisteredGuild(guildConfigForCreateDto.GuildId);
                if (alreadyRegistered != null)
                {
                    throw new GuildAlreadyRegisteredException(guildConfigForCreateDto.GuildId);
                }
            }
            catch (ResourceNotFoundException) { }
            catch (UnregisteredGuildException) { }

            GuildConfig guildConfig = new()
            {
                GuildId = guildConfigForCreateDto.GuildId,
                ModRoles = guildConfigForCreateDto.ModRoles,
                AdminRoles = guildConfigForCreateDto.AdminRoles,
                ModNotificationDM = guildConfigForCreateDto.ModNotificationDM,
                MutedRoles = guildConfigForCreateDto.MutedRoles,
                ModPublicNotificationWebhook = guildConfigForCreateDto.ModPublicNotificationWebhook,
                ModInternalNotificationWebhook = guildConfigForCreateDto.ModInternalNotificationWebhook,
                StrictModPermissionCheck = guildConfigForCreateDto.StrictModPermissionCheck,
                ExecuteWhoisOnJoin = guildConfigForCreateDto.ExecuteWhoisOnJoin,
                PublishModeratorInfo = guildConfigForCreateDto.PublishModeratorInfo,
                PreferredLanguage = guildConfigForCreateDto.PreferredLanguage ?? _config.GetDefaultLanguage(),
                AllowBanAppealAfterDays = guildConfigForCreateDto.AllowBanAppealAfterDays,
                PublicEmbedMode = guildConfigForCreateDto.PublicEmbedMode,
            };

            guildConfig = await GuildConfigRepository.CreateDefault(_serviceProvider).CreateGuildConfig(guildConfig, importExistingBans);

            return StatusCode(201, guildConfig);
        }

        [HttpPut("{guildId}")]
        public async Task<IActionResult> UpdateSpecificItem([FromRoute] ulong guildId, [FromBody] GuildConfigForPutDto newValue)
        {
            await RequirePermission(guildId, DiscordPermission.Admin);
            if (_config.IsDemoModeEnabled())
            {
                if (!(await GetIdentity()).IsSiteAdmin())
                {  // siteadmins can overwrite in demo mode
                    throw new BaseAPIException("Demo mode is enabled. Only site admins can edit guild configs.", APIError.NotAllowedInDemoMode);
                }
            }

            GuildConfig guildConfig = await GetRegisteredGuild(guildId);

            guildConfig.ModRoles = newValue.ModRoles;
            guildConfig.AdminRoles = newValue.AdminRoles;
            guildConfig.ModNotificationDM = newValue.ModNotificationDM;
            guildConfig.MutedRoles = newValue.MutedRoles;
            guildConfig.ModInternalNotificationWebhook = newValue.ModInternalNotificationWebhook;
            if (guildConfig.ModInternalNotificationWebhook != null)
            {
                guildConfig.ModInternalNotificationWebhook = guildConfig.ModInternalNotificationWebhook.Replace("discord.com", "discordapp.com");
            }
            guildConfig.ModPublicNotificationWebhook = newValue.ModPublicNotificationWebhook;
            if (guildConfig.ModPublicNotificationWebhook != null)
            {
                guildConfig.ModPublicNotificationWebhook = guildConfig.ModPublicNotificationWebhook.Replace("discord.com", "discordapp.com");
            }
            guildConfig.StrictModPermissionCheck = newValue.StrictModPermissionCheck;
            guildConfig.ExecuteWhoisOnJoin = newValue.ExecuteWhoisOnJoin;
            guildConfig.PublishModeratorInfo = newValue.PublishModeratorInfo;
            guildConfig.PreferredLanguage = newValue.PreferredLanguage ?? _config.GetDefaultLanguage();
            guildConfig.AllowBanAppealAfterDays = newValue.AllowBanAppealAfterDays;
            guildConfig.PublicEmbedMode = newValue.PublicEmbedMode;

            return Ok(await GuildConfigRepository.CreateDefault(_serviceProvider).UpdateGuildConfig(guildConfig));
        }
    }
}