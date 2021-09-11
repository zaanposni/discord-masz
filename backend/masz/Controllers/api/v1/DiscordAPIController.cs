using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using masz.Dtos.UserAPIResponses;
using masz.Exceptions;
using masz.Models;
using masz.Models.Views;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace masz.Controllers.api.v1
{
    [ApiController]
    [Authorize]
    [Route("api/v1/discord")]
    public class DiscordAPIController : SimpleController
    {
        private readonly ILogger<DiscordAPIController> _logger;

        public DiscordAPIController(ILogger<DiscordAPIController> logger, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _logger = logger;
        }

        [HttpGet("users/@me")]
        public async Task<IActionResult> GetUser()
        {
            Identity identity = await this.GetIdentity();
            DiscordUser currentUser = identity.GetCurrentUser();
            List<DiscordGuild> currentUserGuilds = identity.GetCurrentUserGuilds();

            List<DiscordGuildView> memberGuilds = new List<DiscordGuildView>();
            List<DiscordGuildView> modGuilds = new List<DiscordGuildView>();
            List<DiscordGuildView> adminGuilds = new List<DiscordGuildView>();
            List<DiscordGuildView> bannedGuilds = new List<DiscordGuildView>();
            bool siteAdmin = _config.Value.SiteAdminDiscordUserIds.Contains(currentUser.Id) || identity is TokenIdentity;

            if (identity is DiscordOAuthIdentity)
            {
                List<GuildConfig> registeredGuilds = await _database.SelectAllGuildConfigs();

                foreach (GuildConfig guild in registeredGuilds)
                {
                    DiscordGuild userGuild = currentUserGuilds.FirstOrDefault(x => x.Id == guild.GuildId);
                    if (userGuild != null)
                    {
                        userGuild = await _discordAPI.FetchGuildInfo(userGuild.Id, CacheBehavior.Default);
                        if (userGuild != null)
                        {
                            if (await identity.HasModRoleOrHigherOnGuild(guild.GuildId))
                            {
                                if (await identity.HasAdminRoleOnGuild(guild.GuildId))
                                {
                                    adminGuilds.Add(new DiscordGuildView(userGuild));
                                } else
                                {
                                    modGuilds.Add(new DiscordGuildView(userGuild));
                                }
                            } else {
                                memberGuilds.Add(new DiscordGuildView(userGuild));
                            }
                        }
                    } else
                    {
                        if (await _discordAPI.GetGuildUserBan(guild.GuildId, currentUser.Id, CacheBehavior.Default) != null)
                        {
                            bannedGuilds.Add(new DiscordGuildView(await _discordAPI.FetchGuildInfo(guild.GuildId, CacheBehavior.Default)));
                        }
                    }
                }
            }

            return Ok(new APIUser(memberGuilds, bannedGuilds,  modGuilds, adminGuilds, currentUser, siteAdmin));
        }

        [HttpGet("users/{userid}")]
        public async Task<IActionResult> GetSpecificUser([FromRoute] ulong userid)
        {
            var DiscordUser = await _discordAPI.FetchUserInfo(userid, CacheBehavior.OnlyCache);
            if (DiscordUser != null)
            {
                return Ok(new DiscordUserView(DiscordUser));
            }
            return NotFound();
        }

        [HttpGet("guilds/{guildid}")]
        public async Task<IActionResult> GetSpecificGuild([FromRoute] ulong guildid)
        {
            DiscordGuild guild = await _discordAPI.FetchGuildInfo(guildid, CacheBehavior.Default);
            if (guild != null)
            {
                return Ok(new DiscordGuildView(guild));
            }
            return NotFound();
        }

        [HttpGet("guilds/{guildid}/channels")]
        public async Task<IActionResult> GetAllGuildChannels([FromRoute] ulong guildid)
        {
            var channels = await _discordAPI.FetchGuildChannels(guildid, CacheBehavior.Default);
            if (channels != null)
            {
                return Ok(channels);
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
                return Ok(members.Select(x => new DiscordUserView(x)).ToList());
            }
            return NotFound();
        }

        [HttpGet("guilds")]
        public async Task<IActionResult> GetAllGuilds()
        {
            Identity identity = await this.GetIdentity();
            return Ok(identity.GetCurrentUserGuilds().Select(x => new DiscordGuildView(x)).ToList());
        }
    }
}