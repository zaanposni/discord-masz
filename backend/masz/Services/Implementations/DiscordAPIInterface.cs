using masz.Dtos.DiscordAPIResponses;
using masz.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace masz.Services
{
    public class DiscordAPIInterface : IDiscordAPIInterface
    {        
        private string discordBaseUrl => "https://discord.com/api";
        private readonly ILogger<DiscordAPIInterface> logger;
        private readonly IOptions<InternalConfig> config;
        private readonly string botToken;
        private Dictionary<string, CacheApiResponse> cache = new Dictionary<string, CacheApiResponse>();
        private RestClient restClient;
        private Dictionary<string, DiscordApiRatelimit> ratelimitCache = new Dictionary<string, DiscordApiRatelimit>();

        public DiscordAPIInterface() {  }
        public DiscordAPIInterface(ILogger<DiscordAPIInterface> logger, IOptions<InternalConfig> config)
        {
            this.logger = logger;
            this.config = config;
            this.botToken = config.Value.DiscordBotToken;

            restClient = new RestClient(discordBaseUrl);
        }

        private async Task WaitForRatelimit(string path)
        {
            if (this.ratelimitCache.ContainsKey(path)) {
                if (this.ratelimitCache[path].Remaining == 0) {
                    while (DateTime.UtcNow <= this.ratelimitCache[path].ResetsAt) {
                        await Task.Delay(25);
                    }
                }
            }
        }

        public async Task<List<Ban>> GetGuildBans(string guildId, CacheBehavior cacheBehavior)
        {
            string path = $"/guilds/{guildId}/bans";
            if (cacheBehavior == CacheBehavior.OnlyCache) {
                if (this.cache.ContainsKey(path)) {
                    return JsonConvert.DeserializeObject<List<Ban>>(this.cache[path].Content);
                } else {
                    return new List<Ban>();
                }
            }
            if (this.cache.ContainsKey(path) && cacheBehavior == CacheBehavior.Default) {
                if (this.cache[path].ExpiresAt > DateTime.Now) {
                    return JsonConvert.DeserializeObject<List<Ban>>(this.cache[path].Content);
                }
                this.cache.Remove(path);
            }
            await this.WaitForRatelimit($"/guilds/{guildId}/bans");

            var request = new RestRequest(Method.GET);
            request.Resource = path;
            request.AddHeader("Authorization", "Bot " + botToken);

            var response = await restClient.ExecuteAsync<List<Guild>>(request);
            if (response.IsSuccessful)
            {
                this.ratelimitCache[path] = new DiscordApiRatelimit(response);
                this.cache[path] = new CacheApiResponse(response.Content);
                List<Ban> returnList = JsonConvert.DeserializeObject<List<Ban>>(response.Content);
                foreach (Ban ban in returnList)
                {
                    this.cache[$"{path}/{ban.User.Id}"] = new CacheApiResponse(JsonConvert.SerializeObject(ban));
                    this.cache[$"/users/{ban.User.Id}"] = new CacheApiResponse(JsonConvert.SerializeObject(ban.User));
                }
                return returnList;
            }
            logger.LogError($"{response.Request.Method} {path}: {response.StatusCode} - {response.Content}");
            if (this.cache.ContainsKey(path) && cacheBehavior == CacheBehavior.IgnoreButCacheOnError) {
                return JsonConvert.DeserializeObject<List<Ban>>(this.cache[path].Content);
            }
            return null;
        }
        public async Task<Ban> GetGuildUserBan(string guildId, string userId, CacheBehavior cacheBehavior)
        {
            string path = $"/guilds/{guildId}/bans/{userId}";
            if (cacheBehavior == CacheBehavior.OnlyCache) {
                if (this.cache.ContainsKey(path)) {
                    return new Ban(this.cache[path].Content);
                } else {
                    return null;
                }
            }
            if (this.cache.ContainsKey(path) && cacheBehavior == CacheBehavior.Default) {
                if (this.cache[path].ExpiresAt > DateTime.Now) {
                    return new Ban(this.cache[path].Content);
                }
                this.cache.Remove(path);
            }
            await this.WaitForRatelimit($"/guilds/{guildId}/bans");

            var request = new RestRequest(Method.GET);
            request.Resource = path;
            request.AddHeader("Authorization", "Bot " + botToken);

            var response = await restClient.ExecuteAsync<Ban>(request);
            if (response.IsSuccessful)
            {
                this.ratelimitCache[$"/guilds/{guildId}/bans"] = new DiscordApiRatelimit(response);
                this.cache[path] = new CacheApiResponse(response.Content);
                Ban ban = new Ban(response.Content);
                this.cache[$"/users/{ban.User.Id}"] = new CacheApiResponse(JsonConvert.SerializeObject(ban.User));
                return ban;
            } else {
                if (response.StatusCode == HttpStatusCode.NotFound) {
                    this.cache.Remove(path);  // no longer banned
                }
            }
            
            logger.LogError($"{response.Request.Method} {path}: {response.StatusCode} - {response.Content}");
            if (this.cache.ContainsKey(path) && cacheBehavior == CacheBehavior.IgnoreButCacheOnError) {
                return new Ban(this.cache[path].Content);
            }
            return null;
        }

        public async Task<User> FetchUserInfo(string userId, CacheBehavior cacheBehavior)
        {
            string path = $"/users/{userId}";
            if (cacheBehavior == CacheBehavior.OnlyCache) {
                if (this.cache.ContainsKey(path)) {
                    return new User(this.cache[path].Content);
                } else {
                    return null;
                }
            }
            if (this.cache.ContainsKey(path) && cacheBehavior == CacheBehavior.Default) {
                if (this.cache[path].ExpiresAt > DateTime.Now) {
                    return new User(this.cache[path].Content);
                }
                this.cache.Remove(path);
            }
            await this.WaitForRatelimit("/users");

            var request = new RestRequest(Method.GET);
            request.Resource = path;
            request.AddHeader("Authorization", "Bot " + botToken);

            var response = await restClient.ExecuteAsync<User>(request);
            if (response.IsSuccessful)
            {
                this.ratelimitCache["/users"] = new DiscordApiRatelimit(response);
                this.cache[path] = new CacheApiResponse(response.Content);

                return new User(response.Content);
            }
            logger.LogError($"{response.Request.Method} {path}: {response.StatusCode} - {response.Content}");
            if (this.cache.ContainsKey(path) && cacheBehavior == CacheBehavior.IgnoreButCacheOnError) {
                return new User(this.cache[path].Content);
            }
            return null;
        }

        public async Task<List<GuildMember>> FetchGuildMembers(string guildId, CacheBehavior cacheBehavior)
        {
            string path = $"/guilds/{guildId}/members";
            if (cacheBehavior == CacheBehavior.OnlyCache) {
                if (this.cache.ContainsKey(path)) {
                    return JsonConvert.DeserializeObject<List<GuildMember>>(this.cache[path].Content);
                } else {
                    return new List<GuildMember>();
                }
            }
            if (this.cache.ContainsKey(path) && cacheBehavior == CacheBehavior.Default) {
                if (this.cache[path].ExpiresAt > DateTime.Now) {
                    return JsonConvert.DeserializeObject<List<GuildMember>>(this.cache[path].Content);
                }
                this.cache.Remove(path);
            }            

            List<GuildMember> members = new List<GuildMember>();
            string lastUserId = null;
            do {
                await this.WaitForRatelimit(path);

                var request = new RestRequest(Method.GET);
                request.Resource = path;
                request.AddHeader("Authorization", "Bot " + botToken);
                request.AddQueryParameter("limit", "1000");
                if (!String.IsNullOrEmpty(lastUserId)) {
                    request.AddQueryParameter("after", lastUserId);
                }

                var response = await restClient.ExecuteAsync<List<GuildMember>>(request);
                if (response.IsSuccessful) {
                    this.ratelimitCache[path] = new DiscordApiRatelimit(response);

                    List<GuildMember> newMembers = JsonConvert.DeserializeObject<List<GuildMember>>(response.Content);
                    foreach (GuildMember item in newMembers)
                    {
                        this.cache[$"/guilds/{guildId}/members/{item.User.Id}"] = new CacheApiResponse(JsonConvert.SerializeObject(item));
                        this.cache[$"/users/{item.User.Id}"] = new CacheApiResponse(JsonConvert.SerializeObject(item.User));
                        lastUserId = item.User.Id;
                        members.Add(item);
                    }
                } else {
                    logger.LogError($"{response.Request.Method} {path}: {response.StatusCode} - {response.Content}");
                    if (this.cache.ContainsKey(path) && cacheBehavior == CacheBehavior.IgnoreButCacheOnError) {
                        return JsonConvert.DeserializeObject<List<GuildMember>>(this.cache[path].Content);
                    }
                    return members;
                }
            } while(members.Count != 0 && members.Count % 1000 == 0);

            this.cache[path] = new CacheApiResponse(JsonConvert.SerializeObject(members));
            return members;
        }

        public async Task<User> FetchCurrentUserInfo(string token, CacheBehavior cacheBehavior)
        {
            string path = $"/users/{token}";
            if (cacheBehavior == CacheBehavior.OnlyCache) {
                if (this.cache.ContainsKey(path)) {
                    return new User(this.cache[path].Content);
                } else {
                    return null;
                }
            }
            if (this.cache.ContainsKey(path) && cacheBehavior == CacheBehavior.Default) {
                if (this.cache[path].ExpiresAt > DateTime.Now) {
                    return new User(this.cache[path].Content);
                }
                this.cache.Remove(path);
            }
            await this.WaitForRatelimit(path);

            var request = new RestRequest(Method.GET);
            request.Resource = "/users/@me";
            request.AddHeader("Authorization", "Bearer " + token);

            var response = await restClient.ExecuteAsync<User>(request);
            if (response.IsSuccessful)
            {
                User returnUser = new User(response.Content);
                this.ratelimitCache[path] = new DiscordApiRatelimit(response);
                this.cache[path] = new CacheApiResponse(response.Content);
                this.cache[$"/users/{returnUser.Id}"] = new CacheApiResponse(JsonConvert.SerializeObject(returnUser));

                return returnUser;
            }
            logger.LogError($"{response.Request.Method} {path}: {response.StatusCode} - {response.Content}");
            if (this.cache.ContainsKey(path) && cacheBehavior == CacheBehavior.IgnoreButCacheOnError) {
                return new User(this.cache[path].Content);
            }
            return null;
        }

        public async Task<User> FetchCurrentBotInfo(CacheBehavior cacheBehavior)
        {
            string path = $"/users/@me";
            if (cacheBehavior == CacheBehavior.OnlyCache) {
                if (this.cache.ContainsKey(path)) {
                    return new User(this.cache[path].Content);
                } else {
                    return null;
                }
            }
            if (this.cache.ContainsKey(path) && cacheBehavior == CacheBehavior.Default) {
                if (this.cache[path].ExpiresAt > DateTime.Now) {
                    return new User(this.cache[path].Content);
                }
                this.cache.Remove(path);
            }
            await this.WaitForRatelimit(path);

            var request = new RestRequest(Method.GET);
            request.Resource = path;
            request.AddHeader("Authorization", "Bot " + botToken);

            var response = await restClient.ExecuteAsync<User>(request);
            if (response.IsSuccessful)
            {
                User returnUser = new User(response.Content);
                this.ratelimitCache[path] = new DiscordApiRatelimit(response);
                this.cache[path] = new CacheApiResponse(response.Content);
                this.cache[$"/users/{returnUser.Id}"] = new CacheApiResponse(JsonConvert.SerializeObject(returnUser));

                return returnUser;
            }
            logger.LogError($"{response.Request.Method} {path}: {response.StatusCode} - {response.Content}");
            if (this.cache.ContainsKey(path) && cacheBehavior == CacheBehavior.IgnoreButCacheOnError) {
                return new User(this.cache[path].Content);
            }
            return null;
        }

        public async Task<List<Channel>> FetchGuildChannels(string guildId, CacheBehavior cacheBehavior)
        {
            string path = $"/guilds/{guildId}/channels";
            if (cacheBehavior == CacheBehavior.OnlyCache) {
                if (this.cache.ContainsKey(path)) {
                    return JsonConvert.DeserializeObject<List<Channel>>(this.cache[path].Content);
                } else {
                    return new List<Channel>();
                }
            }
            if (this.cache.ContainsKey(path) && cacheBehavior == CacheBehavior.Default) {
                if (this.cache[path].ExpiresAt > DateTime.Now) {
                    return JsonConvert.DeserializeObject<List<Channel>>(this.cache[path].Content);
                }
                this.cache.Remove(path);
            }
            await this.WaitForRatelimit(path);

            var request = new RestRequest(Method.GET);
            request.Resource = path;
            request.AddHeader("Authorization", "Bot " + botToken);

            var response = await restClient.ExecuteAsync<List<Guild>>(request);
            if (response.IsSuccessful)
            {
                this.ratelimitCache[path] = new DiscordApiRatelimit(response);
                this.cache[path] = new CacheApiResponse(response.Content);
                return JsonConvert.DeserializeObject<List<Channel>>(response.Content);
            }
            logger.LogError($"{response.Request.Method} {path}: {response.StatusCode} - {response.Content}");
            if (this.cache.ContainsKey(path) && cacheBehavior == CacheBehavior.IgnoreButCacheOnError) {
                return JsonConvert.DeserializeObject<List<Channel>>(this.cache[path].Content);
            }
            return null;
        }

        public async Task<Guild> FetchGuildInfo(string guildId, CacheBehavior cacheBehavior)
        {
            string path = $"/guilds/{guildId}";
            if (cacheBehavior == CacheBehavior.OnlyCache) {
                if (this.cache.ContainsKey(path)) {
                    return new Guild(this.cache[path].Content);
                } else {
                    return null;
                }
            }
            if (this.cache.ContainsKey(path) && cacheBehavior == CacheBehavior.Default) {
                if (this.cache[path].ExpiresAt > DateTime.Now) {
                    return new Guild(this.cache[path].Content);
                }
                this.cache.Remove(path);
            }
            await this.WaitForRatelimit(path);

            var request = new RestRequest(Method.GET);
            request.Resource = path;
            request.AddHeader("Authorization", "Bot " + botToken);

            var response = await restClient.ExecuteAsync<Guild>(request);
            if (response.IsSuccessful)
            {
                this.ratelimitCache[path] = new DiscordApiRatelimit(response);
                this.cache[path] = new CacheApiResponse(response.Content);
                return new Guild(response.Content);
            }
            logger.LogError($"{response.Request.Method} {path}: {response.StatusCode} - {response.Content}");
            if (this.cache.ContainsKey(path) && cacheBehavior == CacheBehavior.IgnoreButCacheOnError) {
                return new Guild(this.cache[path].Content);
            }
            return null;
        }

        public async Task<List<Guild>> FetchGuildsOfCurrentUser(string token, CacheBehavior cacheBehavior)
        {
            string path = $"/users/{token}/guilds";
            if (cacheBehavior == CacheBehavior.OnlyCache) {
                if (this.cache.ContainsKey(path)) {
                    return JsonConvert.DeserializeObject<List<Guild>>(this.cache[path].Content);
                } else {
                    return new List<Guild>();
                }
            }
            if (this.cache.ContainsKey(path)) {
                if (this.cache[path].ExpiresAt > DateTime.Now) {
                    return JsonConvert.DeserializeObject<List<Guild>>(this.cache[path].Content);
                }
                this.cache.Remove(path);
            }
            await this.WaitForRatelimit(path);

            var request = new RestRequest(Method.GET);
            request.Resource = "/users/@me/guilds";
            request.AddHeader("Authorization", "Bearer " + token);

            var response = await restClient.ExecuteAsync<List<Guild>>(request);
            if (response.IsSuccessful)
            {
                this.ratelimitCache[path] = new DiscordApiRatelimit(response);
                this.cache[path] = new CacheApiResponse(response.Content);
                return JsonConvert.DeserializeObject<List<Guild>>(response.Content);
            }
            logger.LogError($"{response.Request.Method} {path}: {response.StatusCode} - {response.Content}");
            if (this.cache.ContainsKey(path) && cacheBehavior == CacheBehavior.IgnoreButCacheOnError) {
                return JsonConvert.DeserializeObject<List<Guild>>(this.cache[path].Content);
            }
            return null;
        }

        public async Task<GuildMember> FetchMemberInfo(string guildId, string userId, CacheBehavior cacheBehavior)
        {
            string path = $"/guilds/{guildId}/members/{userId}";
            string rateLimitPath = $"/guilds/{guildId}/members";
            if (cacheBehavior == CacheBehavior.OnlyCache) {
                if (this.cache.ContainsKey(path)) {
                    return new GuildMember(this.cache[path].Content);
                } else {
                    return null;
                }
            }
            if (this.cache.ContainsKey(path) && cacheBehavior == CacheBehavior.Default) {
                if (this.cache[path].ExpiresAt > DateTime.Now) {
                    return new GuildMember(this.cache[path].Content);
                }
                this.cache.Remove(path);
            }
            await this.WaitForRatelimit(rateLimitPath);

            var request = new RestRequest(Method.GET);
            request.Resource = path;
            request.AddHeader("Authorization", "Bot " + botToken);

            var response = await restClient.ExecuteAsync<GuildMember>(request);
            if (response.IsSuccessful)
            {
                GuildMember returnMember = new GuildMember(response.Content);
                this.ratelimitCache[rateLimitPath] = new DiscordApiRatelimit(response);
                this.cache[path] = new CacheApiResponse(response.Content);
                this.cache[$"/users/{returnMember.User.Id}"] = new CacheApiResponse(JsonConvert.SerializeObject(returnMember.User));
                return new GuildMember(response.Content);
            }
            logger.LogError($"{response.Request.Method} {path}: {response.StatusCode} - {response.Content}");
            if (this.cache.ContainsKey(path) && cacheBehavior == CacheBehavior.IgnoreButCacheOnError) {
                return new GuildMember(this.cache[path].Content);
            }
            return null;
        }

        public Task<Message> GetDiscordMessage(string channelId, string messageId, CacheBehavior cacheBehavior)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ValidateUserToken(string token)
        {
            string path = $"/users/{token}";
            await this.WaitForRatelimit(path);

            var request = new RestRequest(Method.GET);
            request.Resource = "/users/@me";
            request.AddHeader("Authorization", "Bearer " + token);

            var response = await restClient.ExecuteAsync<User>(request);

            if (response.IsSuccessful)
            {
                User returnUser = new User(response.Content);
                this.ratelimitCache[path] = new DiscordApiRatelimit(response);
                this.cache[path] = new CacheApiResponse(response.Content);
                this.cache[$"/users/{returnUser.Id}"] = new CacheApiResponse(JsonConvert.SerializeObject(returnUser));

                return true;
            } else {                
                logger.LogError($"{response.Request.Method} {path}: {response.StatusCode} - {response.Content}");
                return false;
            }
        }

        public async Task<bool> BanUser(string guildId, string userId)
        {
            string rateLimitPath = $"/guilds/{guildId}/bans";
            string path = $"/guilds/{guildId}/bans/{userId}";
            await this.WaitForRatelimit(rateLimitPath);

            var request = new RestRequest(Method.PUT);
            request.Resource = path;
            request.AddHeader("Authorization", "Bot " + botToken);

            var response = await restClient.ExecuteAsync(request);
            if (response.IsSuccessful)
            {
                this.ratelimitCache[rateLimitPath] = new DiscordApiRatelimit(response);
                return true;
            }
            logger.LogError($"{response.Request.Method} {path}: {response.StatusCode} - {response.Content}");
            return false;
        }

        public async Task<bool> UnBanUser(string guildId, string userId)
        {
            string rateLimitPath = $"/guilds/{guildId}/bans";
            await this.WaitForRatelimit(rateLimitPath);

            var request = new RestRequest(Method.DELETE);
            request.Resource = $"/guilds/{guildId}/bans/{userId}";
            request.AddHeader("Authorization", "Bot " + botToken);

            var response = await restClient.ExecuteAsync(request);
            if (response.IsSuccessful)
            {
                this.ratelimitCache[rateLimitPath] = new DiscordApiRatelimit(response);
                return true;
            }
            logger.LogError($"{response.Request.Method} {rateLimitPath}: {response.StatusCode} - {response.Content}");
            return false;
        }

        public async Task<bool> GrantGuildUserRole(string guildId, string userId, string roleId)
        {
            string rateLimitPath = $"/guilds/{guildId}/members/roles";
            await this.WaitForRatelimit(rateLimitPath);

            var request = new RestRequest(Method.PUT);
            request.Resource = $"/guilds/{guildId}/members/{userId}/roles/{roleId}";
            request.AddHeader("Authorization", "Bot " + botToken);

            var response = await restClient.ExecuteAsync(request);
            if (response.IsSuccessful)
            {
                this.ratelimitCache[rateLimitPath] = new DiscordApiRatelimit(response);
                return true;
            }
            logger.LogError($"{response.Request.Method} {rateLimitPath}: {response.StatusCode} - {response.Content}");
            return false;
        }

        public async Task<bool> RemoveGuildUserRole(string guildId, string userId, string roleId)
        {
            string rateLimitPath = $"/guilds/{guildId}/members/roles";
            await this.WaitForRatelimit(rateLimitPath);

            var request = new RestRequest(Method.DELETE);
            request.Resource = $"/guilds/{guildId}/members/{userId}/roles/{roleId}";
            request.AddHeader("Authorization", "Bot " + botToken);

            var response = await restClient.ExecuteAsync(request);
            if (response.IsSuccessful)
            {
                this.ratelimitCache[rateLimitPath] = new DiscordApiRatelimit(response);
                return true;
            }
            logger.LogError($"{response.Request.Method} {rateLimitPath}: {response.StatusCode} - {response.Content}");
            return false;
        }

        public async Task<bool> KickGuildUser(string guildId, string userId)
        {
            string rateLimitPath = $"/guilds/{guildId}/members";
            await this.WaitForRatelimit(rateLimitPath);

            var request = new RestRequest(Method.DELETE);
            request.Resource = $"/guilds/{guildId}/members/{userId}";
            request.AddHeader("Authorization", "Bot " + botToken);

            var response = await restClient.ExecuteAsync(request);
            if (response.IsSuccessful)
            {
                this.ratelimitCache[rateLimitPath] = new DiscordApiRatelimit(response);
                return true;
            }
            logger.LogError($"{response.Request.Method} {rateLimitPath}: {response.StatusCode} - {response.Content}");
            return false;
        }

        public async Task<Channel> CreateDmChannel(string userId)
        {
            string path = $"/users/@me/channels/{userId}";
            if (this.cache.ContainsKey(path)) {
                if (this.cache[path].ExpiresAt > DateTime.Now) {
                    return new Channel(this.cache[path].Content);
                }
                this.cache.Remove(path);
            }
            await this.WaitForRatelimit(path);

            var request = new RestRequest(Method.POST);
            request.Resource = $"/users/@me/channels";
            request.AddHeader("Authorization", "Bot " + botToken);
            request.AddJsonBody(new {recipient_id = userId});

            var response = await restClient.ExecuteAsync<Channel>(request);
            if (response.IsSuccessful)
            {
                this.ratelimitCache[path] = new DiscordApiRatelimit(response);
                this.cache[path] = new CacheApiResponse(response.Content);
                return new Channel(response.Content);
            }
            logger.LogError($"{response.Request.Method} {path}: {response.StatusCode} - {response.Content}");
            return null;
        }

        public async Task<bool> SendMessage(string channelId, string content)
        {
            string path = $"/channels/{channelId}/messages";
            await this.WaitForRatelimit(path);

            var request = new RestRequest(Method.POST);
            request.Resource = path;
            request.AddHeader("Authorization", "Bot " + botToken);
            request.AddJsonBody(new {content = content});

            var response = await restClient.ExecuteAsync(request);
            if (response.IsSuccessful)
            {
                this.ratelimitCache[path] = new DiscordApiRatelimit(response);
                return true;
            }
            logger.LogError($"{response.Request.Method} {path}: {response.StatusCode} - {response.Content}");
            return false;
        }

        public async Task<bool> SendEmbedMessage(string channelId, object content)
        {
            string path = $"/channels/{channelId}/messages";
            await this.WaitForRatelimit(path);

            var request = new RestRequest(Method.POST);
            request.Resource = path;
            request.AddHeader("Authorization", "Bot " + botToken);
            request.AddJsonBody(new {embed = content});

            var response = await restClient.ExecuteAsync(request);
            if (response.IsSuccessful)
            {
                this.ratelimitCache[path] = new DiscordApiRatelimit(response);
                return true;
            }
            logger.LogError($"{response.Request.Method} {path}: {response.StatusCode} - {response.Content}");
            return false;
        }

        public async Task<bool> SendDmMessage(string userId, string content)
        {
            Channel channel = await this.CreateDmChannel(userId);
            if (channel == null) {
                return false;
            }

            return await this.SendMessage(channel.Id, content);
        }

        public Dictionary<string, CacheApiResponse> GetCache()
        {
            return this.cache;
        }
    }
}
