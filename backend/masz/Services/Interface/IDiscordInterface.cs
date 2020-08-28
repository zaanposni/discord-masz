using masz.Dtos.DiscordAPIResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace masz.Services
{
    public interface IDiscordInterface
    {
        string discordBaseUrl { get; }

        /// <summary>
        /// Checks if the discord personal access token is valid.
        /// https://discord.com/developers/docs/resources/user#get-current-user
        /// </summary>
        /// <param name="token">discord personal access token to use to authenticate against the discord api</param>
        /// <returns>True if the request could be authenticated and thus the token is valid.</returns>
        Task<bool> ValidateUserToken(string token);

        /// <summary>
        /// Returns information of user by his id
        /// https://discord.com/developers/docs/resources/user#get-user
        /// </summary>
        /// <param name="token">discord personal access token to use to authenticate against the discord api</param>
        /// <returns>User object if found.</returns>
        Task<User> FetchCurrentUserInfo(string token);

        /// <summary>
        /// Returns information of user by his id
        /// https://discord.com/developers/docs/resources/user#get-user
        /// </summary>
        /// <param name="userId">discord user id to fetch</param>
        /// <returns>User object if found.</returns>
        Task<User> FetchUserInfo(string userId);

        /// <summary>
        /// Returns information of a discord guild member bis his id and the guilds
        /// https://discord.com/developers/docs/resources/guild#get-guild-member
        /// </summary>
        /// <param name="guildId">discord guild id to fetch</param>
        /// <param name="userId">discord user id to fetch</param>
        /// <returns>User object if found.</returns>
        Task<GuildMember> FetchMemberInfo(string guildId, string userId);

        /// <summary>
        /// Returns information of user by his id
        /// https://discord.com/developers/docs/resources/guild#get-guild
        /// </summary>
        /// <param name="guildId">discord guild id to fetch</param>
        /// <returns>User object if found.</returns>
        Task<Guild> FetchGuildInfo(string guildId);

        /// <summary>
        /// This method returns a list of IDs for all guilds an user defined by his personal access token is member of.
        /// https://discord.com/developers/docs/resources/user#get-current-user-guilds
        /// </summary>
        /// <param name="token">discord personal access token to use to authenticate against the discord api</param>
        /// <returns>List of guildId snowflakes that the user is member of.</returns>
        Task<List<Guild>> FetchGuildsOfCurrentUser(string token);

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
