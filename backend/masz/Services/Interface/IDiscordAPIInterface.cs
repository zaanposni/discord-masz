using masz.Dtos.DiscordAPIResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace masz.Services
{
    public interface IDiscordAPIInterface
    {
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
        /// Returns information of current bot
        /// https://discord.com/developers/docs/resources/user#get-user
        /// </summary>
        /// <returns>User object if found.</returns>
        Task<User> FetchCurrentBotInfo();

        /// <summary>
        /// Returns information of user by his id
        /// Respects discord rate limit
        /// https://discord.com/developers/docs/resources/user#get-user
        /// </summary>
        /// <param name="userId">discord user id to fetch</param>
        /// <param name="breakCache">if it should ignore/break the cached user</param>
        /// <returns>User object if found.</returns>
        Task<User> FetchUserInfo(string userId, bool breakCache = false);

        /// <summary>
        /// Returns information of a discord guild member bis his id and the guilds
        /// https://discord.com/developers/docs/resources/guild#get-guild-member
        /// </summary>
        /// <param name="guildId">discord guild id to fetch</param>
        /// <param name="userId">discord user id to fetch</param>
        /// <param name="breakCache">if it should ignore/break the cached user</param>
        /// <returns>User object if found.</returns>
        Task<GuildMember> FetchMemberInfo(string guildId, string userId, bool breakCache = false);
        Task<List<GuildMember>> FetchGuildMembers(string guildId, bool breakCache = false);

        /// <summary>
        /// Returns information of guild channels by guild id
        ///https://discord.com/developers/docs/resources/guild#get-guild-channels
        /// </summary>
        /// <param name="guildId">discord guild id to fetch</param>
        /// <returns>List of guild channels.</returns>

        Task<List<Channel>> FetchGuildChannels(string guildId);
        /// <summary>
        /// Returns information of guild by its id
        /// https://discord.com/developers/docs/resources/guild#get-guild
        /// </summary>
        /// <param name="guildId">discord guild id to fetch</param>
        /// <returns>Guild object if found.</returns>
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
        /// This method checks if a defined user is banned on a guild and returns the ban object or null
        /// https://discord.com/developers/docs/resources/guild#get-guild-ban
        /// </summary>
        /// <param name="guildId">guild to check bans on</param>
        /// <param name="userId">user to check bans for</param>
        /// <returns>returns the ban object or null if not found</returns>
        Task<Ban> GetGuildUserBan(string guildId, string userId);

        Task<bool> BanUser(string guildId, string userId);
        Task<bool> UnBanUser(string guildId, string userId);
        Task<bool> GrantGuildUserRole(string guildId, string userId, string roleId);
        Task<bool> RemoveGuildUserRole(string guildId, string userId, string roleId);
        Task<bool> KickGuildUser(string guildId, string userId);
        Task<Channel> CreateDmChannel(string userId);
        Task<bool> SendMessage(string channelId, string content);
        Task<bool> SendEmbedMessage(string channelId, object content);
        Task<bool> SendDmMessage(string userId, string content);
    }
}
