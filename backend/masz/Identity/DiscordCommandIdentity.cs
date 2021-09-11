using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using masz.Exceptions;
using masz.Repositories;
using masz.Services;
using Microsoft.Extensions.Logging;

namespace masz.Models
{
    public class DiscordCommandIdentity : Identity
    {
        private readonly ILogger<DiscordCommandIdentity> _logger;
        protected readonly IDiscordBot _discordBot;
        private Dictionary<ulong, DiscordMember> GuildMemberships = new Dictionary<ulong, DiscordMember>();
        public async static Task<DiscordCommandIdentity> Create(DiscordUser user, IServiceProvider serviceProvider)
        {
            IDatabase database = serviceProvider.GetService(typeof (IDatabase)) as IDatabase;
            IDiscordAPIInterface discordAPI = serviceProvider.GetService(typeof (IDiscordAPIInterface)) as IDiscordAPIInterface;

            List<GuildConfig> guildConfigs = await database.SelectAllGuildConfigs();
            List<DiscordGuild> guilds = new List<DiscordGuild>();
            foreach (GuildConfig guildConfig in guildConfigs)
            {
                if ((await discordAPI.FetchMemberInfo(guildConfig.GuildId, user.Id, CacheBehavior.Default)) != null)
                {
                    guilds.Add(await discordAPI.FetchGuildInfo(guildConfig.GuildId, CacheBehavior.Default));
                }
            }
            return new DiscordCommandIdentity(serviceProvider, user, guilds);
        }
        private DiscordCommandIdentity (IServiceProvider serviceProvider, DiscordUser currentUser, List<DiscordGuild> userGuilds) : base(currentUser.Id.ToString(), serviceProvider)
        {
            this.currentUser = currentUser;
            this.currentUserGuilds = userGuilds;
            this._logger = serviceProvider.GetService(typeof(ILogger<DiscordCommandIdentity>)) as ILogger<DiscordCommandIdentity>;
            this._discordBot = serviceProvider.GetService(typeof(IDiscordBot)) as IDiscordBot;
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

            GuildConfig guildConfig;
            try
            {
                guildConfig = await GuildConfigRepository.CreateDefault(_serviceProvider).GetGuildConfig(guildId);
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

            IDatabase database = GetDatabase();

            GuildConfig guildConfig;
            try
            {
                guildConfig = await GuildConfigRepository.CreateDefault(_serviceProvider).GetGuildConfig(guildId);
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

        public override bool IsSiteAdmin()
        {
            if (currentUser == null) {
                return false;
            }
            return _config.GetSiteAdmins().Contains(currentUser.Id);
        }
    }
}