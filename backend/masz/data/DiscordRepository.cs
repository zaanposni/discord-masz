using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using masz.Dtos.DiscordAPIResponses;
using masz.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;

namespace masz.data
{
    public class DiscordRepository : IDiscordRepository
    {
        public string discordBaseUrl => "https://discord.com/api";
        private RestClient restClient;
        private readonly string botToken;

        public DiscordRepository(IOptions<InternalConfig> config)
        {
            restClient = new RestClient(discordBaseUrl);
            botToken = config.Value.DiscordBotToken;
        }

        public async Task<bool> DiscordUserHasRoleOnGuild(string guildId, string roleId, string userId)
        {
            var request = new RestRequest(Method.GET);
            request.Resource = "/guilds/" + guildId + "/members/" + userId;
            request.AddHeader("Authorization", "Bot " + botToken);

            var response = await restClient.ExecuteAsync<GuildMember>(request);
            if (response.IsSuccessful)
            {
                var json = JsonConvert.DeserializeObject<GuildMember>(response.Content);
                return json.Roles.Contains(roleId);
            }
            return false;
        }

        public Task<bool> DiscordUserIsBannedOnGuild(string guildId, string userId)
        {
            throw new System.NotImplementedException();
        }

        public async Task<bool> DiscordUserIsMemberOfGuild(string guildId, string token)
        {
            var request = new RestRequest(Method.GET);
            request.Resource = "/users/@me/guilds";
            request.AddHeader("Authorization", "Bearer " + token);

            var response = await restClient.ExecuteAsync<List<UserGuilds>>(request);
            if (response.IsSuccessful)
            {
                var json = JsonConvert.DeserializeObject<List<UserGuilds>>(response.Content);
                return json.Any(x => x.id == guildId);
            }
            return false;
        }

        public Task<Message> GetDiscordMessage(string channelId, string messageId)
        {
            throw new System.NotImplementedException();
        }

        public Task<List<string>> GetGuildsByDiscordUser(string token)
        {
            throw new System.NotImplementedException();
        }

        public async Task<User> ValidateDiscordUserToken(string token)
        {
            var request = new RestRequest(Method.GET);
            request.Resource = "/users/@me";
            request.AddHeader("Authorization", "Bearer " + token);

            var response = await restClient.ExecuteAsync<User>(request);
            if (response.IsSuccessful)
            {
                return new User(response.Content);
            }
            return null;
        }

        public async Task<User> FetchDiscordUserInfo(string userId)
        {
            var request = new RestRequest(Method.GET);
            request.Resource = "/users/" + userId;
            request.AddHeader("Authorization", "Bot " + botToken);

            var response = await restClient.ExecuteAsync<User>(request);
            if (response.IsSuccessful)
            {
                return new User(response.Content);
            }
            return null;
        }
    }
}