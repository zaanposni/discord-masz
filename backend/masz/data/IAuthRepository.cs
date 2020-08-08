using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace masz.data
{
    public interface IAuthRepository
    {
        Task<bool> DiscordUserIsAuthorized(HttpContext user);

        Task<bool> DiscordUserIsAuthorizedServer(ClaimsPrincipal user1, int guildId);
    }
}