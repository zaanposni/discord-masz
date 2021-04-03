using masz.Dtos.DiscordAPIResponses;
using masz.Models;
using masz.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace masz.Models
{
    public abstract class Identity
    {
        public readonly IDiscordAPIInterface discord;
        public DateTime ValidUntil { get; set; }
        public string Token;
        public User DiscordUser;
        public List<Guild> Guilds;
        public Identity (string token, IDiscordAPIInterface discord)
        {
            this.discord = discord;
            this.Token = token;
            this.ValidUntil = DateTime.UtcNow.AddMinutes(15);
        }

        /// <summary>
        /// This method checks if a HttpContext holds a valid Discord user token.
        /// https://discord.com/developers/docs/resources/user#get-current-user
        /// </summary>
        /// <returns>if a request with the users token could be authenticated against hte discord api</returns>
        public abstract Task<bool> IsAuthorized();

        /// <summary>
        /// This method returns the discord user of the current authenticated user in the http context
        /// https://discord.com/developers/docs/resources/user#get-current-user
        /// </summary>
        /// <returns>returns current discord user if the a request against discord API could be authenticated</returns>
        public abstract Task<User> GetCurrentDiscordUser();

        /// <summary>
        /// This method returns all guilds the user is registered on.
        /// https://discord.com/developers/docs/resources/user#get-current-user-guilds
        /// </summary>
        /// <returns>List of guilds the current user is member on.</returns>
        public abstract Task<List<Guild>> GetCurrentGuilds();

        /// <summary>
        /// This method checks if the discord user is member of a specified guild.
        /// https://discord.com/developers/docs/resources/user#get-current-user-guilds
        /// </summary>
        /// <param name="guildId">guild that the user should be member of</param>
        /// <returns>True the user is member of the specified guild.</returns>
        public abstract Task<bool> IsOnGuild(string guildId);

        /// <summary>
        /// This method checksif the discord user is member of a specified guild and has a mod role or higher as they are specified in the database.
        /// https://discord.com/developers/docs/resources/guild#get-guild-member
        /// </summary>
        /// <param name="guildId">guild that the user requestes for</param>
        /// <returns>the guildmember object if the current user could be found on that guild.</returns>
        public abstract Task<GuildMember> GetGuildMembership(string guildId);

        /// <summary>
        /// Checks if the current user has the defined admin role on the defined guild.
        /// </summary>
        /// <param name="guildId">the guild to check on</param>
        /// <returns>True if the user is on this guild and is member of the admin role.</returns>
        public abstract Task<bool> HasAdminRoleOnGuild(string guildId, IDatabase database);

        /// <summary>
        /// Checks if the current user has a defined team role on the defined guild.
        /// </summary>
        /// <param name="guildId">the guild to check on</param>
        /// <returns>True if the user is on this guild and has at least one of the configured roles.</returns>
        public abstract Task<bool> HasModRoleOrHigherOnGuild(string guildId, IDatabase database);
    }
}