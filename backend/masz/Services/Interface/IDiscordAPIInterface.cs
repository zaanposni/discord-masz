using DSharpPlus.Entities;
using masz.Enums;
using masz.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace masz.Services
{
    public interface IDiscordAPIInterface
    {
        Dictionary<string, CacheApiResponse> GetCache();
        void RemoveFromCache(CacheKey key);
        T GetFromCache<T>(CacheKey key);
        void AddOrUpdateCache(CacheKey key, CacheApiResponse response);
        /// <summary>
        /// Returns information of user by his id
        /// https://discord.com/developers/docs/resources/user#get-user
        /// </summary>
        /// <param name="token">discord personal access token to use to authenticate against the discord api</param>
        /// <returns>User object if found.</returns>
        Task<DiscordUser> FetchCurrentUserInfo(string token, CacheBehavior cacheBehavior);

        /// <summary>
        /// Returns information of current bot
        /// https://discord.com/developers/docs/resources/user#get-user
        /// </summary>
        /// <returns>User object if found.</returns>
        DiscordUser GetCurrentBotInfo(CacheBehavior cacheBehavior);
        DiscordApplication GetCurrentApplicationInfo();
        Task<DiscordUser> FetchCurrentBotInfo();

        /// <summary>
        /// Returns information of user by his id
        /// Respects discord rate limit
        /// https://discord.com/developers/docs/resources/user#get-user
        /// </summary>
        /// <param name="userId">discord user id to fetch</param>
        /// <param name="breakCache">if it should ignore/break the cached user</param>
        /// <returns>User object if found.</returns>
        Task<DiscordUser> FetchUserInfo(ulong userId, CacheBehavior cacheBehavior);

        /// <summary>
        /// Returns information of a discord guild member bis his id and the guilds
        /// https://discord.com/developers/docs/resources/guild#get-guild-member
        /// </summary>
        /// <param name="guildId">discord guild id to fetch</param>
        /// <param name="userId">discord user id to fetch</param>
        /// <param name="breakCache">if it should ignore/break the cached user</param>
        /// <returns>User object if found.</returns>
        Task<DiscordMember> FetchMemberInfo(ulong guildId, ulong userId, CacheBehavior cacheBehavior);
        Task<List<DiscordMember>> FetchGuildMembers(ulong guildId, CacheBehavior cacheBehavior);

        /// <summary>
        /// Returns information of guild channels by guild id
        ///https://discord.com/developers/docs/resources/guild#get-guild-channels
        /// </summary>
        /// <param name="guildId">discord guild id to fetch</param>
        /// <returns>List of guild channels.</returns>

        Task<List<DiscordChannel>> FetchGuildChannels(ulong guildId, CacheBehavior cacheBehavior);
        /// <summary>
        /// Returns information of guild by its id
        /// https://discord.com/developers/docs/resources/guild#get-guild
        /// </summary>
        /// <param name="guildId">discord guild id to fetch</param>
        /// <returns>Guild object if found.</returns>
        Task<DiscordGuild> FetchGuildInfo(ulong guildId, CacheBehavior cacheBehavior);

        /// <summary>
        /// This method returns a list of IDs for all guilds a user defined by his personal access token is member of.
        /// https://discord.com/developers/docs/resources/user#get-current-user-guilds
        /// </summary>
        /// <param name="token">discord personal access token to use to authenticate against the discord api</param>
        /// <returns>List of guildId snowflakes that the user is member of.</returns>
        Task<List<DiscordGuild>> FetchGuildsOfCurrentUser(string token, CacheBehavior cacheBehavior);

        /// <summary>
        /// This method fetches a discord guild channel message
        /// https://discord.com/developers/docs/resources/channel#get-channel-message
        /// </summary>
        /// <param name="channelId"></param>
        /// <param name="messageId"></param>
        /// <returns>fetched message or null if not found</returns>
        Task<DiscordMessage> GetDiscordMessage(ulong channelId, ulong messageId, CacheBehavior cacheBehavior);

        /// <summary>
        /// This method checks if a defined user is banned on a guild and returns the ban object or null
        /// https://discord.com/developers/docs/resources/guild#get-guild-ban
        /// </summary>
        /// <param name="guildId">guild to check bans on</param>
        /// <param name="userId">user to check bans for</param>
        /// <returns>returns the ban object or null if not found</returns>
        Task<DiscordBan> GetGuildUserBan(ulong guildId, ulong userId, CacheBehavior cacheBehavior);
        Task<List<DiscordBan>> GetGuildBans(ulong guildId, CacheBehavior cacheBehavior);
        Task<bool> BanUser(ulong guildId, ulong userId);
        Task<bool> UnBanUser(ulong guildId, ulong userId);
        Task<bool> GrantGuildUserRole(ulong guildId, ulong userId, ulong roleId);
        Task<bool> RemoveGuildUserRole(ulong guildId, ulong userId, ulong roleId);
        Task<bool> KickGuildUser(ulong guildId, ulong userId);
        Task<DiscordChannel> CreateDmChannel(ulong userId);
        Task<bool> SendMessage(ulong channelId, string content = null, DiscordEmbed embed = null);
        Task<bool> SendDmMessage(ulong userId, string content);
        Task<bool> SendDmMessage(ulong userId, DiscordEmbed embed);
        Task<bool> ExecuteWebhook(string url, DiscordEmbed embed = null, string content = null);
    }
}
