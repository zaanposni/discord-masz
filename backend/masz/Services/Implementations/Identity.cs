using masz.Dtos.DiscordAPIResponses;
using masz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace masz.Services
{
    public class Identity
    {
        private readonly IDiscordAPIInterface discord;

        public DateTime ValidUntil { get; set;  }

        private string Token;
        private User DiscordUser;
        private List<Guild> Guilds;
        private Dictionary<string, GuildMember> GuildMemberships = new Dictionary<string, GuildMember>();


        public Identity (string token, IDiscordAPIInterface discord)
        {
            this.discord = discord;
            this.Token = token;
            this.ValidUntil = DateTime.Now.AddMinutes(5);
        }

        private async Task LoadBasicDetails()
        {
            this.DiscordUser = await discord.FetchCurrentUserInfo(Token);
            this.Guilds = await discord.FetchGuildsOfCurrentUser(Token);
        }

        private async Task ValidateBasicDetails()
        {
            if (DiscordUser == null)
            {
                await LoadBasicDetails();
            }
        }

        /// <summary>
        /// This method checks if a HttpContext holds a valid Discord user token.
        /// https://discord.com/developers/docs/resources/user#get-current-user
        /// </summary>
        /// <returns>if a request with the users token could be authenticated against hte discord api</returns>
        public async Task<bool> IsAuthorized() 
        {
            await ValidateBasicDetails();
            return DiscordUser != null;
        }

        /// <summary>
        /// This method returns the discord user of the current authenticated user in the http context
        /// https://discord.com/developers/docs/resources/user#get-current-user
        /// </summary>
        /// <returns>returns current discord user if the a request against discord API could be authenticated</returns>
        public async Task<User> GetCurrentDiscordUser()
        {
            await ValidateBasicDetails();
            return DiscordUser;
        }

        /// <summary>
        /// This method returns all guilds the user is registered on.
        /// https://discord.com/developers/docs/resources/user#get-current-user-guilds
        /// </summary>
        /// <returns>List of guilds the current user is member on.</returns>
        public async Task<List<Guild>> GetCurrentGuilds()
        {
            await ValidateBasicDetails();
            if (Guilds != null)
            {
                return Guilds;
            }
            else
            {
                Guilds = await discord.FetchGuildsOfCurrentUser(Token);
                return Guilds;
            }
        }

        /// <summary>
        /// This method checks if the discord user is member of a specified guild.
        /// https://discord.com/developers/docs/resources/user#get-current-user-guilds
        /// </summary>
        /// <param name="guildId">guild that the user should be member of</param>
        /// <returns>True the user is member of the specified guild.</returns>
        public async Task<bool> IsOnGuild(string guildId)
        {
            await ValidateBasicDetails();
            if (Guilds != null)
            {
                return Guilds.Any(x => x.Id == guildId);
            } else
            {
                Guilds = await discord.FetchGuildsOfCurrentUser(Token);
                if (Guilds == null)
                {
                    return false;
                }
                return Guilds.Any(x => x.Id == guildId);
            }
        }

        /// <summary>
        /// This method checksif the discord user is member of a specified guild and has a mod role or higher as they are specified in the database.
        /// https://discord.com/developers/docs/resources/guild#get-guild-member
        /// </summary>
        /// <param name="guildId">guild that the user requestes for</param>
        /// <returns>the guildmember object if the current user could be found on that guild.</returns>
        public async Task<GuildMember> GetGuildMembership(string guildId)
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
                    GuildMember guildMember = await discord.FetchMemberInfo(guildId, DiscordUser.Id);
                    GuildMemberships.Add(guildId, guildMember);
                    return guildMember;
                }
            }
        }

        /// <summary>
        /// Checks if the current user has a defined team role on the defined guild.
        /// </summary>
        /// <param name="guildId">the guild to check on</param>
        /// <returns>True if the user is on this guild and has at least one of the configured roles.</returns>
        public async Task<bool> HasModRoleOrHigherOnGuild(string guildId, IDatabase database)
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
            return guildMember.Roles.Contains(guildConfig.ModRoleId) || guildMember.Roles.Contains(guildConfig.AdminRoleId);
        }
    }
}
