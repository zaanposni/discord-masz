using Discord;
using MASZ.Data;
using MASZ.Enums;
using MASZ.Exceptions;
using MASZ.Repositories;
using MASZ.Services;

namespace MASZ.Models
{
    public class DiscordCommandIdentity : Identity
    {
        protected readonly DiscordBot _discordBot;
        private readonly Dictionary<ulong, IGuildUser> GuildMemberships = new();

        public async static Task<DiscordCommandIdentity> Create(IUser user, IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            Database database = scope.ServiceProvider.GetRequiredService<Database>();
            DiscordAPIInterface discordAPI = serviceProvider.GetRequiredService<DiscordAPIInterface>();

            List<GuildConfig> guildConfigs = await database.SelectAllGuildConfigs();
            List<UserGuild> guilds = new();
            foreach (GuildConfig guildConfig in guildConfigs)
            {
                var member = await discordAPI.FetchMemberInfo(guildConfig.GuildId, user.Id, CacheBehavior.Default);

                if (member != null)
                {
                    guilds.Add(new UserGuild(member));
                }
            }
            return new DiscordCommandIdentity(serviceProvider, user, guilds);
        }
        private DiscordCommandIdentity(IServiceProvider serviceProvider, IUser currentUser, List<UserGuild> userGuilds) : base(currentUser.Id.ToString(), serviceProvider)
        {
            this.currentUser = currentUser;
            currentUserGuilds = userGuilds;
            _discordBot = serviceProvider.GetRequiredService<DiscordBot>();
        }
        public override async Task<IGuildUser> GetGuildMembership(ulong guildId)
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
                IGuildUser guildMember = await _discordAPI.FetchMemberInfo(guildId, currentUser.Id, CacheBehavior.Default);
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
            if (!IsOnGuild(guildId))
            {
                return false;
            }

            GuildConfig guildConfig;
            try
            {
                using var scope = _serviceProvider.CreateScope();
                guildConfig = await GuildConfigRepository.CreateDefault(scope.ServiceProvider).GetGuildConfig(guildId);
            }
            catch (ResourceNotFoundException)
            {
                return false;
            }

            IGuildUser guildMember = await GetGuildMembership(guildId);
            if (guildMember == null)
            {
                return false;
            }
            if (guildMember.Guild.OwnerId == guildMember.Id)
            {
                return true;
            }

            // check for role
            return guildMember.RoleIds.Where(x => guildConfig.AdminRoles.Contains(x)).Any();
        }

        public override async Task<bool> HasModRoleOrHigherOnGuild(ulong guildId)
        {
            if (!IsOnGuild(guildId))
            {
                return false;
            }

            GuildConfig guildConfig;
            try
            {
                using var scope = _serviceProvider.CreateScope();
                guildConfig = await GuildConfigRepository.CreateDefault(scope.ServiceProvider).GetGuildConfig(guildId);
            }
            catch (ResourceNotFoundException)
            {
                return false;
            }

            IGuildUser guildMember = await GetGuildMembership(guildId);
            if (guildMember == null)
            {
                return false;
            }
            if (guildMember.Guild.OwnerId == guildMember.Id)
            {
                return true;
            }
            return guildMember.RoleIds.Any(x => guildConfig.AdminRoles.Contains(x) ||
                                                guildConfig.ModRoles.Contains(x));
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
            }
            else
            {
                return false;
            }
        }

        public override bool IsSiteAdmin()
        {
            if (currentUser == null)
            {
                return false;
            }
            return _config.GetSiteAdmins().Contains(currentUser.Id);
        }

        public override void RemoveGuildMembership(ulong guildId)
        {
            currentUserGuilds.RemoveAll(x => x.Id == guildId);
            GuildMemberships.Remove(guildId);
        }

        public override void AddGuildMembership(IGuildUser member)
        {
            if (!currentUserGuilds.Any(x => x.Id == member.Guild.Id))
            {
                currentUserGuilds.Add(new UserGuild(member));
            }
            GuildMemberships[member.Guild.Id] = member;
        }

        public override void UpdateGuildMembership(IGuildUser member)
        {
            if (!currentUserGuilds.Any(x => x.Id == member.Guild.Id))
            {
                currentUserGuilds.Add(new UserGuild(member));
            }
            GuildMemberships[member.Guild.Id] = member;
        }
    }
}