using DSharpPlus.Entities;
using masz.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace masz.Models
{
    public class DiscordOAuthIdentity : Identity
    {
        private Dictionary<ulong, DiscordMember> GuildMemberships = new Dictionary<ulong, DiscordMember>();
        public async static Task<DiscordOAuthIdentity> Create(string token, IServiceProvider serviceProvider)
        {
            IDiscordAPIInterface api = serviceProvider.GetService(typeof(IDiscordAPIInterface)) as IDiscordAPIInterface;
            DiscordUser user = await api.FetchCurrentUserInfo(token, CacheBehavior.IgnoreButCacheOnError);
            List<DiscordGuild> guilds = await api.FetchGuildsOfCurrentUser(token, CacheBehavior.IgnoreButCacheOnError);

            return new DiscordOAuthIdentity(token, serviceProvider, user, guilds);
        }

        private DiscordOAuthIdentity (string token, IServiceProvider serviceProvider, DiscordUser currentUser, List<DiscordGuild> userGuilds) : base(token, serviceProvider)
        {
            this.currentUser = currentUser;
            this.currentUserGuilds = userGuilds;
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

            IDatabase database = GetDatabase();

            // Get guild config from database
            GuildConfig guildConfig = await database.SelectSpecificGuildConfig(guildId);
            if (guildConfig == null)
            {
                return false;
            }

            DiscordMember guildMember = await GetGuildMembership(guildId);
            if (guildMember == null) {
                return false;
            }

            // check for role
            return guildMember.Roles.Where(x => guildConfig.AdminRoles.Contains(x.Id.ToString())).Any();
        }

        public override async Task<bool> HasModRoleOrHigherOnGuild(ulong guildId)
        {
            if (! IsOnGuild(guildId))
            {
                return false;
            }

            IDatabase database = GetDatabase();

            // Get guild config from database
            GuildConfig guildConfig = await database.SelectSpecificGuildConfig(guildId);
            if (guildConfig == null)
            {
                return false;
            }

            DiscordMember guildMember = await GetGuildMembership(guildId);
            if (guildMember == null) {
                return false;
            }

            return guildMember.Roles.Where(x => guildConfig.AdminRoles.Contains(x.Id.ToString()) ||
                                                guildConfig.ModRoles.Contains(x.Id.ToString())).Any();
        }

        public override bool IsSiteAdmin()
        {
            if (currentUser == null) {
                return false;
            }
            return _config.Value.SiteAdminDiscordUserIds.Contains(currentUser.Id);
        }
    }
}
