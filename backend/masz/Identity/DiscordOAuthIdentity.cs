using DSharpPlus.Entities;
using masz.Exceptions;
using masz.Repositories;
using masz.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace masz.Models
{
    public class DiscordOAuthIdentity : Identity
    {
        private readonly ILogger<DiscordOAuthIdentity> _logger;
        private Dictionary<ulong, DiscordMember> GuildMemberships = new Dictionary<ulong, DiscordMember>();
        public async static Task<DiscordOAuthIdentity> Create(string token, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory)
        {
            IDiscordAPIInterface api = serviceProvider.GetService(typeof(IDiscordAPIInterface)) as IDiscordAPIInterface;
            DiscordUser user = await api.FetchCurrentUserInfo(token, CacheBehavior.IgnoreButCacheOnError);
            List<DiscordGuild> guilds = await api.FetchGuildsOfCurrentUser(token, CacheBehavior.IgnoreButCacheOnError);

            return new DiscordOAuthIdentity(token, serviceProvider, user, guilds, serviceScopeFactory);
        }

        private DiscordOAuthIdentity (string token, IServiceProvider serviceProvider, DiscordUser currentUser, List<DiscordGuild> userGuilds, IServiceScopeFactory serviceScopeFactory) : base(token, serviceProvider, serviceScopeFactory)
        {
            this.currentUser = currentUser;
            this.currentUserGuilds = userGuilds;
            this._logger = serviceProvider.GetService(typeof(ILogger<DiscordOAuthIdentity>)) as ILogger<DiscordOAuthIdentity>;
        }

        public override bool IsAuthorized()
        {
            return currentUser != null;
        }

        public override bool IsOnGuild(ulong guildId)
        {
            if (currentUser != null)
            {
                return currentUserGuilds.Any(x => x.Id == guildId);
            } else
            {
                return false;
            }
        }
        public override async Task<DiscordMember> GetGuildMembership(ulong guildId)
        {
            if (GuildMemberships.ContainsKey(guildId))
            {
                return GuildMemberships[guildId];
            }
            else
            {
                if (currentUser == null)
                {
                    return null;
                }
                DiscordMember guildMember = await _discordAPI.FetchMemberInfo(guildId, currentUser.Id, CacheBehavior.Default);
                if (guildMember == null)
                {
                    return null;
                }
                GuildMemberships[guildId] = guildMember;
                return guildMember;
            }
        }
        public override async Task<bool> HasAdminRoleOnGuild(ulong guildId)
        {
            if (! IsOnGuild(guildId))
            {
                return false;
            }

            GuildConfig guildConfig;
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    guildConfig = await GuildConfigRepository.CreateDefault(scope.ServiceProvider).GetGuildConfig(guildId);
                }
            } catch (ResourceNotFoundException)
            {
                return false;
            }

            DiscordMember guildMember = await GetGuildMembership(guildId);
            if (guildMember == null) {
                return false;
            }

            // check for role
            return guildMember.Roles.Where(x => guildConfig.AdminRoles.Contains(x.Id)).Any();
        }
        public override async Task<bool> HasModRoleOrHigherOnGuild(ulong guildId)
        {
            if (! IsOnGuild(guildId))
            {
                return false;
            }

            GuildConfig guildConfig;
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    guildConfig = await GuildConfigRepository.CreateDefault(scope.ServiceProvider).GetGuildConfig(guildId);
                }
            } catch (ResourceNotFoundException)
            {
                return false;
            }

            DiscordMember guildMember = await GetGuildMembership(guildId);
            if (guildMember == null) {
                return false;
            }
            return guildMember.Roles.Any(x => guildConfig.AdminRoles.Contains(x.Id) ||
                                                guildConfig.ModRoles.Contains(x.Id));
        }
        public override bool IsSiteAdmin()
        {
            if (currentUser == null) {
                return false;
            }
            return _config.GetSiteAdmins().Contains(currentUser.Id);
        }
    }
}
