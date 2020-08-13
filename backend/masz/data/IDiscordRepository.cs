using System.Collections.Generic;
using System.Threading.Tasks;
using masz.Dtos.DiscordAPIResponses;

namespace masz.data
{
    public interface IDiscordRepository
    {
        string discordBaseUrl { get; }

        /// <summary>
        /// Checks if the discord personal access token is valid.
        /// https://discord.com/developers/docs/resources/user#get-current-user
        /// </summary>
        /// <param name="token">discord personal access token to use to authenticate against the discord api</param>
        /// <returns>True if the request could be authenticated and thus the token is valid.</returns>
        Task<User> ValidateDiscordUserToken(string token);

        /// <summary>
        /// Returns information of user by his id
        /// https://discord.com/developers/docs/resources/user#get-user
        /// </summary>
        /// <param name="userId">discord user id to fetch</param>
        /// <returns>User object if found.</returns>
        Task<User> FetchDiscordUserInfo(string userId);

        /// <summary>
        /// This method returns a list of IDs for all guilds an user defined by his personal access token is member of.
        /// https://discord.com/developers/docs/resources/user#get-current-user-guilds
        /// </summary>
        /// <param name="token">discord personal access token to use to authenticate against the discord api</param>
        /// <returns>List of guildId snowflakes that the user is member of.</returns>
        Task<List<string>> GetGuildsByDiscordUser(string token);
        
        /// <summary>
        /// This method checks if an user defined by his personal access token is member of a defined guild
        /// https://discord.com/developers/docs/resources/user#get-current-user-guilds
        /// </summary>
        /// <param name="guildId">the discord guild to check for</param>
        /// <param name="token">discord personal access token to use to authenticate against the discord api</param>
        /// <returns>True if the a request against discord API could be authenticated and the user is member of the specified guild.</returns>
        Task<bool> DiscordUserIsMemberOfGuild(string guildId, string token);

        /// <summary>
        /// This method checks if the defined user has the defined role on a defined guild
        /// https://discord.com/developers/docs/resources/guild#get-guild-member
        /// </summary>
        /// <param name="guildId">the guild to perform checks on</param>
        /// <param name="roleId">the role to check membership for</param>
        /// <param name="userId">the user to check membership for</param>
        /// <returns>True if the user is member of the defined role.</returns>
        Task<bool> DiscordUserHasRoleOnGuild(string guildId, string roleId, string userId);

        /// <summary>
        /// This method fetches a discord guild channel message
        /// https://discord.com/developers/docs/resources/channel#get-channel-message
        /// </summary>
        /// <param name="channelId"></param>
        /// <param name="messageId"></param>
        /// <returns>fetched message or null if not found</returns>
        Task<Message> GetDiscordMessage(string channelId, string messageId);

        /// <summary>
        /// This method checks if a defined user is banned on a guild
        /// https://discord.com/developers/docs/resources/guild#get-guild-ban
        /// </summary>
        /// <param name="guildId">guild to check bans on</param>
        /// <param name="userId">user to check bans for</param>
        /// <returns>if the defined user is banned on the defined guild</returns>
        Task<bool> DiscordUserIsBannedOnGuild(string guildId, string userId);
    }
}