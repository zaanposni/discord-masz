using masz.Dtos.DiscordAPIResponses;
using masz.Models;
using masz.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace masz.Models
{
    public class DiscordIdentity : Identity
    {
        private Dictionary<string, GuildMember> GuildMemberships = new Dictionary<string, GuildMember>();
        public DiscordIdentity (string token, IDiscordAPIInterface discord) : base(token, discord) { }

        private async Task LoadBasicDetails()
        {
            this.DiscordUser = await discord.FetchCurrentUserInfo(Token, CacheBehavior.Default);
            this.Guilds = await discord.FetchGuildsOfCurrentUser(Token, CacheBehavior.Default);
        }

        private async Task ValidateBasicDetails()
        {
            if (DiscordUser == null)
            {
                await LoadBasicDetails();
            }
        }

        public override async Task<bool> IsAuthorized() 
        {
            await ValidateBasicDetails();
            return DiscordUser != null;
        }

        public override async Task<User> GetCurrentDiscordUser()
        {
            await ValidateBasicDetails();
            return DiscordUser;
        }

        public override async Task<List<Guild>> GetCurrentGuilds()
        {
            await ValidateBasicDetails();
            if (Guilds != null)
            {
                return Guilds;
            }
            else
            {
                Guilds = await discord.FetchGuildsOfCurrentUser(Token, CacheBehavior.Default);
                return Guilds;
            }
        }

        public override async Task<bool> IsOnGuild(string guildId)
        {
            await ValidateBasicDetails();
            if (Guilds != null)
            {
                return Guilds.Any(x => x.Id == guildId);
            } else
            {
                Guilds = await discord.FetchGuildsOfCurrentUser(Token, CacheBehavior.Default);
                if (Guilds == null)
                {
                    return false;
                }
                return Guilds.Any(x => x.Id == guildId);
            }
        }
        public override async Task<GuildMember> GetGuildMembership(string guildId)
        {
            if (GuildMemberships.ContainsKey(guildId))
            {
                return GuildMemberships[guildId];
            } 
            else
            {
                await ValidateBasicDetails();
                if (DiscordUser == null)
                {
                    return null;
                }
                else
                {
                    GuildMember guildMember = await discord.FetchMemberInfo(guildId, DiscordUser.Id, CacheBehavior.Default);
                    GuildMemberships[guildId] = guildMember;
                    return guildMember;
                }
            }
        }

        public override async Task<bool> HasAdminRoleOnGuild(string guildId, IDatabase database)
        {
            if (!await IsOnGuild(guildId))
            {
                return false;
            }

            // Get guild config from database
            GuildConfig guildConfig = await database.SelectSpecificGuildConfig(guildId);
            if (guildConfig == null)
            {
                return false;
            }

            GuildMember guildMember = await GetGuildMembership(guildId);
            if (guildMember == null) {
                return false;
            }

            // check for role
            return guildMember.Roles.Intersect(guildConfig.AdminRoles).Any();
        }

        public override async Task<bool> HasModRoleOrHigherOnGuild(string guildId, IDatabase database)
        {
            if (!await IsOnGuild(guildId))
            {
                return false;
            }

            // Get guild config from database
            GuildConfig guildConfig = await database.SelectSpecificGuildConfig(guildId);
            if (guildConfig == null)
            {
                return false;
            }

            GuildMember guildMember = await GetGuildMembership(guildId);
            if (guildMember == null) {
                return false;
            }

            return guildMember.Roles.Intersect(guildConfig.ModRoles).Any() || guildMember.Roles.Intersect(guildConfig.AdminRoles).Any();
        }
    }
}
