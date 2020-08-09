using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using masz.Dtos.DiscordAPIResponses;
using masz.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;

namespace masz.data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ILogger<AuthRepository> logger;
        private readonly RestClient client;
        private readonly IOptions<InternalConfig> config;
        private readonly DataContext dbContext;

        public AuthRepository(ILogger<AuthRepository> logger, IOptions<InternalConfig> config, DataContext context) {
            this.logger = logger;
            this.config = config;
            this.dbContext = context;            
            this.client = new RestClient("https://discordapp.com/api");
        }

        public async Task<string> GetDiscordUserId(HttpContext context)
        {
            var token = await context.GetTokenAsync("access_token");

            var request = new RestRequest(Method.GET);
            request.Resource = "/users/@me";
            request.AddHeader("Authorization", "Bearer " + token);

            var response = client.Execute<User>(request);
            if (response.IsSuccessful)
            {
                var json = JsonConvert.DeserializeObject<User>(response.Content);
                return json.id;
            }
            return null;
        }

        public async Task<bool> DiscordUserIsAuthorized(HttpContext context)
        {
            var token = await context.GetTokenAsync("access_token");

            var request = new RestRequest(Method.GET);
            request.Resource = "/users/@me";
            request.AddHeader("Authorization", "Bearer " + token);

            var response = client.Execute(request);
            return HttpStatusCode.OK.Equals(response.StatusCode);
        }

        public async Task<bool> DiscordUserIsOnServer(HttpContext context, string guildId)
        {
            string token = await context.GetTokenAsync("access_token");

            var request = new RestRequest(Method.GET);
            request.Resource = "/users/@me/guilds";
            request.AddHeader("Authorization", "Bearer " + token);

            var response = client.Execute<List<UserGuilds>>(request);
            if (response.IsSuccessful)
            {
                var json = JsonConvert.DeserializeObject<List<UserGuilds>>(response.Content);
                return json.Any(x => x.id == guildId);
            }
            return false;
        }

        public async Task<bool> DiscordUserHasModRoleOrHigherOnGuild(HttpContext context, string guildId)
        {
            if (! await DiscordUserIsOnServer(context, guildId)) 
            {
                return false;
            }

            string userId = await GetDiscordUserId(context);

            // Get ModRole from database
            GuildConfig modGuild = await dbContext.GuildConfigs.FirstOrDefaultAsync(x => x.GuildId == guildId);

            if (modGuild == null)
             {
                return false;      
            }      

            var request = new RestRequest(Method.GET);
            request.Resource = "/guilds/" + guildId + "/members/" + userId;
            request.AddHeader("Authorization", "Bot " + config.Value.DiscordBotToken);

            var response = client.Execute<GuildMember>(request);
            if (response.IsSuccessful)
            {
                var json = JsonConvert.DeserializeObject<GuildMember>(response.Content);
                return json.roles.Contains(modGuild.ModRoleId) || json.roles.Contains(modGuild.AdminRoleId);
            }
            return false;
        }
    }
}