using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace masz.data
{
    public interface IAuthRepository
    {
        /// <summary>
        /// This method returns the discord user id of the current authenticated user in the http context
        /// https://discord.com/developers/docs/resources/user#get-current-user
        /// </summary>
        /// <param name="context">current http context to check</param>
        /// <returns>if a request with the users token could be authenticated against hte discord api</returns>
        Task<string> GetDiscordUserId(HttpContext context);

        /// <summary>
        /// This method checks if a HttpContext holds a valid Discord user token.
        /// https://discord.com/developers/docs/resources/user#get-current-user
        /// </summary>
        /// <param name="context">current http context to check</param>
        /// <returns>if a request with the users token could be authenticated against hte discord api</returns>
        Task<bool> DiscordUserIsAuthorized(HttpContext context);

        /// <summary>
        /// This method checks if a HttpContext holds a valid Discord user token and if the discord user is member of a specified guild.
        /// https://discord.com/developers/docs/resources/user#get-current-user-guilds
        /// </summary>
        /// <param name="context">current http context to check</param>
        /// <param name="guildId">guild that the user should be member of</param>
        /// <returns>True if the a request against discord API could be authenticated and the user is member of the specified guild.</returns>
        Task<bool> DiscordUserIsAuthorizedServer(HttpContext context, int guildId);

        /// <summary>
        /// This method checks if a HttpContext holds a valid Discord user token and if the discord user is member of a specified guild and has a specified role.
        /// https://discord.com/developers/docs/resources/guild#get-guild-member
        /// </summary>
        /// <param name="context"></param>
        /// <param name="guildId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        Task<bool> DiscordUserHasRoleOnGuild(HttpContext context, int guildId, int roleId);
    }
}