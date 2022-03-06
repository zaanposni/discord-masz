using Discord;
using MASZ.Dtos.UserAPIResponses;
using MASZ.Enums;
using MASZ.Exceptions;
using MASZ.Models;
using MASZ.Models.Views;
using MASZ.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MASZ.Controllers.api.v1
{
    [ApiController]
    [Authorize]
    [Route("api/v1/discord")]
    public class DiscordAPIController : SimpleController
    {
        public DiscordAPIController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpGet("users/@me")]
        public async Task<IActionResult> GetUser()
        {
            Identity identity = await GetIdentity();
            IUser currentUser = identity.GetCurrentUser();
            List<UserGuild> currentUserGuilds = identity.GetCurrentUserGuilds();

            List<DiscordGuildView> memberGuilds = new();
            List<DiscordGuildView> modGuilds = new();
            List<DiscordGuildView> adminGuilds = new();
            List<DiscordGuildView> bannedGuilds = new();
            bool siteAdmin = _config.GetSiteAdmins().Contains(currentUser.Id) || identity is TokenIdentity;

            if (identity is DiscordOAuthIdentity)
            {
                List<GuildConfig> registeredGuilds = await GuildConfigRepository.CreateDefault(_serviceProvider).GetAllGuildConfigs();

                foreach (GuildConfig guild in registeredGuilds)
                {
                    UserGuild userGuild = currentUserGuilds.FirstOrDefault(x => x.Id == guild.GuildId);
                    if (userGuild != null || await AppealRepository.CreateDefault(_serviceProvider).UserHasConfirmedAppeal(guild.GuildId, currentUser.Id))
                    {
                        IGuild userGuildFetched = _discordAPI.FetchGuildInfo(guild.GuildId, CacheBehavior.Default);
                        if (userGuildFetched != null)
                        {
                            if (await identity.HasModRoleOrHigherOnGuild(guild.GuildId))
                            {
                                if (await identity.HasAdminRoleOnGuild(guild.GuildId))
                                {
                                    adminGuilds.Add(new DiscordGuildView(userGuildFetched));
                                }
                                else
                                {
                                    modGuilds.Add(new DiscordGuildView(userGuildFetched));
                                }
                            }
                            else
                            {
                                memberGuilds.Add(new DiscordGuildView(userGuildFetched));
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            _discordAPI.GetFromCache<IBan>(CacheKey.GuildBan(guild.GuildId, currentUser.Id));
                            bannedGuilds.Add(new DiscordGuildView(_discordAPI.FetchGuildInfo(guild.GuildId, CacheBehavior.Default)));
                            continue;
                        }
                        catch (NotFoundInCacheException) { }

                        try
                        {
                            if (await AppealRepository.CreateDefault(_serviceProvider).UserHasPendingOrDeclinedAppeal(guild.GuildId, currentUser.Id))
                            {
                                bannedGuilds.Add(new DiscordGuildView(_discordAPI.FetchGuildInfo(guild.GuildId, CacheBehavior.Default)));
                                continue;
                            }
                        }
                        catch (NotFoundInCacheException) { }

                        if ((await ModCaseRepository.CreateWithBotIdentity(_serviceProvider).GetCasesForGuildAndUser(guild.GuildId, currentUser.Id))
                                        .Where(x => x.PunishmentType == PunishmentType.Ban && x.PunishmentActive)
                                        .OrderByDescending(x => x.CreatedAt)
                                        .FirstOrDefault() != null)
                        {
                            bannedGuilds.Add(new DiscordGuildView(_discordAPI.FetchGuildInfo(guild.GuildId, CacheBehavior.Default)));
                            continue;
                        }
                    }
                }
            }

            return Ok(new APIUser(memberGuilds, bannedGuilds, modGuilds, adminGuilds, currentUser, siteAdmin));
        }

        [HttpGet("users/{userid}")]
        public async Task<IActionResult> GetSpecificUser([FromRoute] ulong userid)
        {
            var IUser = await _discordAPI.FetchUserInfo(userid, CacheBehavior.OnlyCache);
            if (IUser != null)
            {
                return Ok(DiscordUserView.CreateOrDefault(IUser));
            }
            return NotFound();
        }

        [HttpGet("guilds/{guildid}")]
        public IActionResult GetSpecificGuild([FromRoute] ulong guildid)
        {
            IGuild guild = _discordAPI.FetchGuildInfo(guildid, CacheBehavior.Default);
            if (guild != null)
            {
                return Ok(new DiscordGuildView(guild));
            }
            return NotFound();
        }

        [HttpGet("guilds/{guildid}/channels")]
        public IActionResult GetAllGuildChannels([FromRoute] ulong guildid)
        {
            var channels = _discordAPI.FetchGuildChannels(guildid, CacheBehavior.Default);
            if (channels != null)
            {
                return Ok(channels.Select(x => new DiscordChannelView(x)));
            }
            return NotFound();
        }

        [HttpGet("guilds/{guildId}/members")]
        public async Task<IActionResult> GetGuildMembers([FromRoute] ulong guildId)
        {
            await GetRegisteredGuild(guildId);  // Endpoint only available for registered guilds.

            var members = await _discordAPI.FetchGuildMembers(guildId, CacheBehavior.OnlyCache);
            if (members != null)
            {
                return Ok(members.Select(x => DiscordUserView.CreateOrDefault(x)));
            }
            return NotFound();
        }

        [HttpGet("guilds")]
        public async Task<IActionResult> GetAllGuilds()
        {
            Identity identity = await GetIdentity();
            return Ok(identity.GetCurrentUserGuilds().Select(x => new DiscordGuildView(x)));
        }
    }
}