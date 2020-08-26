using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using masz.Models;

namespace masz.data
{
    public interface IAuthRepository
    {
        /// <summary>
        /// This method returns the discord personal access token that can be obtained with the token received during the oauth process
        /// </summary>
        /// <param name="context">current http context to check</param>
        /// <returns>discord personal access token for authenticated user</returns>
        Task<string> GetDiscordUserToken(HttpContext context);

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
        Task<bool> DiscordUserIsOnServer(HttpContext context, string guildId);

        /// <summary>
        /// This method checks if a HttpContext holds a valid Discord user token and if the discord user is member of a specified guild and has a mod role or higher as they are specified in the database.
        /// https://discord.com/developers/docs/resources/guild#get-guild-member
        /// </summary>
        /// <param name="context">current http context to check</param>
        /// <param name="guildId">guild that the user requestes for</param>
        /// <returns>True if the request against discord API could be authenticated and the user has at least on specified role or higher.</returns>
        Task<bool> DiscordUserHasModRoleOrHigherOnGuild(HttpContext context, string guildId);

        /// <summary>
        /// This method checks if a HttpContext holds a valid Discord user token and if the discord user is authorized to view a defined mocase. Only mods / admins and the user the case is about are allowed to view it.
        /// https://discord.com/developers/docs/resources/guild#get-guild-member
        /// </summary>
        /// <param name="context">current http context to check</param>
        /// <param name="modCase">the case to validate authorization for</param>
        /// <returns>True if the request against discord API could be authenticated and the user is authorized ot view this case.</returns>
        Task<bool> DiscordUserIsAuthorizedToViewModCase(HttpContext context, ModCase modCase);
    }
}