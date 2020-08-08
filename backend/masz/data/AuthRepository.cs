using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace masz.data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ILogger<AuthRepository> logger;
        private readonly RestClient client;

        public AuthRepository(ILogger<AuthRepository> logger) {
            this.logger = logger;
            this.client = new RestClient("https://discordapp.com/api");
        }

        public async Task<bool> DiscordUserIsAuthorized(HttpContext context)
        {
            var test = await context.GetTokenAsync("access_token");

            logger.LogInformation(test.ToString());

            var request = new RestRequest(Method.GET);
            request.Resource = "/users/@me";
            request.AddHeader("Authorization", "Bearer " + test.ToString());

            var response = client.Execute(request);
            return HttpStatusCode.OK.Equals(response.StatusCode);
        }

        public Task<bool> DiscordUserIsAuthorizedServer(ClaimsPrincipal user1, int guildId)
        {
            // request.Resource("/guilds");
            throw new System.NotImplementedException();
        }
    }
}