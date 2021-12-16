using Discord;
using Discord.Rest;
using Discord.Webhook;
using Discord.WebSocket;
using MASZ.Enums;
using MASZ.Exceptions;
using MASZ.Models;

namespace MASZ.Services
{
    public class DiscordAPIInterface
    {
        private readonly ILogger<DiscordAPIInterface> _logger;
        private readonly InternalConfiguration _config;
        private readonly DiscordSocketClient _client;
        private readonly DiscordRestClient _discordRestClient;
        private readonly Dictionary<string, CacheApiResponse> _cache = new();

        public DiscordAPIInterface() { }
        public DiscordAPIInterface(ILogger<DiscordAPIInterface> logger, InternalConfiguration config, DiscordSocketClient client)
        {
            _logger = logger;
            _config = config;
            _client = client;
            _discordRestClient = new DiscordRestClient();
            _discordRestClient.LoginAsync(
                TokenType.Bot,
                _config.GetBotToken()
            );
        }

        public async Task<DiscordRestClient> GetOAuthClient(string token)
        {
            var client = new DiscordRestClient();

            await client.LoginAsync(TokenType.Bearer, token);

            return client;
        }

        private T TryGetFromCache<T>(CacheKey cacheKey, CacheBehavior cacheBehavior)
        {
            if (cacheBehavior == CacheBehavior.OnlyCache)
            {
                if (_cache.ContainsKey(cacheKey.GetValue()))
                {
                    return _cache[cacheKey.GetValue()].GetContent<T>();
                }
                else
                {
                    throw new NotFoundInCacheException(cacheKey.GetValue());
                }
            }
            if (_cache.ContainsKey(cacheKey.GetValue()) && cacheBehavior == CacheBehavior.Default)
            {
                if (!_cache[cacheKey.GetValue()].IsExpired())
                {
                    return _cache[cacheKey.GetValue()].GetContent<T>();
                }
                _cache.Remove(cacheKey.GetValue());
            }
            return default;
        }

        private T FallBackToCache<T>(CacheKey cacheKey, CacheBehavior cacheBehavior)
        {
            if (cacheBehavior != CacheBehavior.IgnoreCache)
            {
                if (_cache.ContainsKey(cacheKey.GetValue()))
                {
                    if (!_cache[cacheKey.GetValue()].IsExpired())
                    {
                        return _cache[cacheKey.GetValue()].GetContent<T>();
                    }
                    _cache.Remove(cacheKey.GetValue());
                }
            }
            return default;
        }

        private void SetCacheValue(CacheKey cacheKey, CacheApiResponse cacheApiResponse)
        {
            _cache[cacheKey.GetValue()] = cacheApiResponse;
        }

        public async Task<List<IBan>> GetGuildBans(ulong guildId, CacheBehavior cacheBehavior)
        {
            // do cache stuff --------------------
            CacheKey cacheKey = CacheKey.GuildBans(guildId);
            List<IBan> bans;
            try
            {
                bans = TryGetFromCache<List<IBan>>(cacheKey, cacheBehavior);
                if (bans != null) return bans;
            }
            catch (NotFoundInCacheException)
            {
                return new List<IBan>();
            }

            // request ---------------------------
            try
            {
                IGuild guild = FetchGuildInfo(guildId, CacheBehavior.Default);
                if (guild == null) return new List<IBan>();
                bans = (await guild.GetBansAsync()).ToList();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to fetch guild bans for guild '{guildId}' from API.");
                return FallBackToCache<List<IBan>>(cacheKey, cacheBehavior);
            }

            // cache -----------------------------
            SetCacheValue(cacheKey, new CacheApiResponse(bans));
            foreach (IBan ban in bans)
            {
                SetCacheValue(CacheKey.User(ban.User.Id), new CacheApiResponse(ban.User));
                SetCacheValue(CacheKey.GuildBan(guildId, ban.User.Id), new CacheApiResponse(ban));
            }
            return bans;
        }

        public async Task<IBan> GetGuildUserBan(ulong guildId, ulong userId, CacheBehavior cacheBehavior)
        {
            // do cache stuff --------------------
            CacheKey cacheKey = CacheKey.GuildBan(guildId, userId);
            IBan ban = null;
            try
            {
                ban = TryGetFromCache<IBan>(cacheKey, cacheBehavior);
                if (ban != null) return ban;
            }
            catch (NotFoundInCacheException)
            {
                return ban;
            }

            // request ---------------------------
            try
            {
                IGuild guild = FetchGuildInfo(guildId, CacheBehavior.Default);
                if (guild == null) return null;
                ban = await guild.GetBanAsync(userId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to fetch guild ban for guild '{guildId}' and user '{userId}' from API.");
                return FallBackToCache<IBan>(cacheKey, cacheBehavior);
            }

            // cache -----------------------------
            SetCacheValue(cacheKey, new CacheApiResponse(ban));
            SetCacheValue(CacheKey.User(ban.User.Id), new CacheApiResponse(ban.User));
            return ban;
        }

        public async Task<IUser> FetchUserInfo(ulong userId, CacheBehavior cacheBehavior)
        {
            // do cache stuff --------------------
            CacheKey cacheKey = CacheKey.User(userId);
            IUser user = null;
            try
            {
                user = TryGetFromCache<IUser>(cacheKey, cacheBehavior);
                if (user != null) return user;
            }
            catch (NotFoundInCacheException)
            {
                return user;
            }

            // request ---------------------------
            try
            {
                user = await _client.GetUserAsync(userId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to fetch user '{userId}' from API.");
                return FallBackToCache<IUser>(cacheKey, cacheBehavior);
            }

            // cache -----------------------------
            SetCacheValue(cacheKey, new CacheApiResponse(user));
            return user;
        }

        public async Task<List<IGuildUser>> FetchGuildMembers(ulong guildId, CacheBehavior cacheBehavior)
        {
            // do cache stuff --------------------
            CacheKey cacheKey = CacheKey.GuildMembers(guildId);
            List<IGuildUser> members;
            try
            {
                members = TryGetFromCache<List<IGuildUser>>(cacheKey, cacheBehavior);
                if (members != null) return members;
            }
            catch (NotFoundInCacheException)
            {
                return new List<IGuildUser>();
            }

            // request ---------------------------
            try
            {
                IGuild guild = FetchGuildInfo(guildId, cacheBehavior);
                if (guild == null) return new List<IGuildUser>();
                members = (await guild.GetUsersAsync()).ToList();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to fetch members for guild '{guildId}' from API.");
                return FallBackToCache<List<IGuildUser>>(cacheKey, cacheBehavior);
            }

            // cache -----------------------------
            foreach (IGuildUser item in members)
            {
                SetCacheValue(CacheKey.User(item.Id), new CacheApiResponse(item));
                SetCacheValue(CacheKey.GuildMember(guildId, item.Id), new CacheApiResponse(item));
            }
            SetCacheValue(cacheKey, new CacheApiResponse(members));
            return members;
        }

        public async Task<IUser> FetchCurrentUserInfo(string token, CacheBehavior cacheBehavior)
        {
            // do cache stuff --------------------
            CacheKey cacheKey = CacheKey.TokenUser(token);
            IUser user = null;
            try
            {
                user = TryGetFromCache<IUser>(cacheKey, cacheBehavior);
                if (user != null) return user;
            }
            catch (NotFoundInCacheException)
            {
                return user;
            }

            // request ---------------------------
            try
            {
                user = (await GetOAuthClient(token)).CurrentUser;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to fetch current user for token '{token}' from API.");
                return FallBackToCache<IUser>(cacheKey, cacheBehavior);
            }

            // cache -----------------------------
            SetCacheValue(cacheKey, new CacheApiResponse(user));
            return user;
        }

        public IUser GetCurrentBotInfo()
        {
            return _client.CurrentUser;
        }

        public async Task<IUser> FetchCurrentBotInfo()
        {
            var client = new DiscordRestClient();

            await client.LoginAsync(TokenType.Bot, _config.GetBotToken());

            return client.CurrentUser;
        }

        public async Task<List<IChannel>> FetchGuildChannels(ulong guildId, CacheBehavior cacheBehavior)
        {
            // do cache stuff --------------------
            CacheKey cacheKey = CacheKey.GuildChannels(guildId);
            List<IChannel> channels;
            try
            {
                channels = TryGetFromCache<List<IChannel>>(cacheKey, cacheBehavior);
                if (channels != null) return channels;
            }
            catch (NotFoundInCacheException)
            {
                return new List<IChannel>();
            }

            // request ---------------------------
            try
            {
                IGuild guild = FetchGuildInfo(guildId, cacheBehavior);
                if (guild == null) return new List<IChannel>();
                channels = (await guild.GetChannelsAsync()).Select(channel => channel as IChannel).ToList();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to fetch guild channels for guild '{guildId}' from API.");
                return FallBackToCache<List<IChannel>>(cacheKey, cacheBehavior);
            }

            // cache -----------------------------
            SetCacheValue(cacheKey, new CacheApiResponse(channels));
            return channels;
        }

        public IGuild FetchGuildInfo(ulong guildId, CacheBehavior cacheBehavior)
        {
            // do cache stuff --------------------
            CacheKey cacheKey = CacheKey.Guild(guildId);
            IGuild guild;
            try
            {
                guild = TryGetFromCache<IGuild>(cacheKey, cacheBehavior);
                if (guild != null) return guild;
            }
            catch (NotFoundInCacheException)
            {
                return null;
            }

            // request ---------------------------
            try
            {
                guild = _client.GetGuild(guildId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to fetch guild '{guildId}' from API.");
                return FallBackToCache<IGuild>(cacheKey, cacheBehavior);
            }

            // cache -----------------------------
            SetCacheValue(cacheKey, new CacheApiResponse(guild));
            return guild;
        }

        public async Task<List<IGuild>> FetchGuildsOfCurrentUser(string token, CacheBehavior cacheBehavior)
        {
            // do cache stuff --------------------
            CacheKey cacheKey = CacheKey.TokenUserGuilds(token);
            List<IGuild> guilds;
            try
            {
                guilds = TryGetFromCache<List<IGuild>>(cacheKey, cacheBehavior);
                if (guilds != null) return guilds;
            }
            catch (NotFoundInCacheException)
            {
                return new List<IGuild>();
            }

            // request ---------------------------
            try
            {
                guilds = (await (await GetOAuthClient(token)).GetGuildsAsync()).Select(guild => guild as IGuild).ToList();  // max 200 guilds
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to fetch guilds of current user for token '{token}' from API.");
                return FallBackToCache<List<IGuild>>(cacheKey, cacheBehavior);
            }

            // cache -----------------------------
            SetCacheValue(cacheKey, new CacheApiResponse(guilds));
            return guilds;
        }

        public async Task<IGuildUser> FetchMemberInfo(ulong guildId, ulong userId, CacheBehavior cacheBehavior)
        {
            // do cache stuff --------------------
            CacheKey cacheKey = CacheKey.GuildMember(guildId, userId);
            IGuildUser member;
            try
            {
                member = TryGetFromCache<IGuildUser>(cacheKey, cacheBehavior);
                if (member != null) return member;
            }
            catch (NotFoundInCacheException)
            {
                return null;
            }

            // request ---------------------------
            try
            {
                IGuild g = _client.GetGuild(guildId);
                member = await g.GetUserAsync(userId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to fetch guild '{guildId}' member '{userId}' from API.");
                return FallBackToCache<IGuildUser>(cacheKey, cacheBehavior);
            }

            // cache -----------------------------
            SetCacheValue(cacheKey, new CacheApiResponse(member));
            SetCacheValue(CacheKey.User(userId), new CacheApiResponse(member));
            return member;
        }

        public Task<IMessage> GetIMessage(ulong channelId, ulong messageId, CacheBehavior cacheBehavior)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> BanUser(ulong guildId, ulong userId)
        {
            try
            {
                IGuild guild = FetchGuildInfo(guildId, CacheBehavior.Default);
                if (guild == null) return false;
                await guild.AddBanAsync(userId, 0);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to ban user '{userId}' from guild '{guildId}'.");
                return false;
            }
            return true;
        }

        public async Task<bool> UnBanUser(ulong guildId, ulong userId)
        {
            try
            {
                IGuild guild = FetchGuildInfo(guildId, CacheBehavior.Default);
                if (guild == null) return false;
                await guild.RemoveBanAsync(userId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to unban user '{userId}' from guild '{guildId}'.");
                return false;
            }
            return true;
        }

        public async Task<bool> GrantGuildUserRole(ulong guildId, ulong userId, ulong roleId)
        {
            // request ---------------------------
            try
            {
                IGuild guild = FetchGuildInfo(guildId, CacheBehavior.Default);
                if (guild == null) return false;
                IGuildUser member = await FetchMemberInfo(guildId, userId, CacheBehavior.Default);
                if (member == null) return false;
                IRole role = guild.Roles.Where(r => r.Id == roleId).FirstOrDefault();
                if (role == null) return false;
                await member.AddRoleAsync(role);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to grant user '{userId}' from guild '{guildId}' role '{roleId}'.");
                return false;
            }
            return true;
        }

        public async Task<bool> RemoveGuildUserRole(ulong guildId, ulong userId, ulong roleId)
        {
            // request ---------------------------
            try
            {
                IGuild guild = FetchGuildInfo(guildId, CacheBehavior.Default);
                if (guild == null) return false;
                IGuildUser member = await FetchMemberInfo(guildId, userId, CacheBehavior.Default);
                if (member == null) return false;
                IRole role = guild.Roles.Where(r => r.Id == roleId).FirstOrDefault();
                if (role == null) return false;
                await member.RemoveRoleAsync(role);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to revoke user '{userId}' from guild '{guildId}' role '{roleId}'.");
                return false;
            }
            return true;
        }

        public async Task<bool> KickGuildUser(ulong guildId, ulong userId)
        {
            // request ---------------------------
            try
            {
                IGuildUser member = await FetchMemberInfo(guildId, userId, CacheBehavior.Default);
                if (member == null) return false;
                await member.KickAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to kick user '{userId}' from guild '{guildId}'.");
                return false;
            }
            return true;
        }

        public async Task<IChannel> CreateDmChannel(ulong userId)
        {
            // do cache stuff --------------------
            CacheKey cacheKey = CacheKey.DMChannel(userId);
            IChannel channel;
            try
            {
                channel = TryGetFromCache<IChannel>(cacheKey, CacheBehavior.Default);
                if (channel != null) return channel;
            }
            catch (NotFoundInCacheException)
            {
                return null;
            }

            // request ---------------------------
            try
            {
                channel = await (await _discordRestClient.GetUserAsync(userId)).CreateDMChannelAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to create dm with user '{userId}'.");
                return FallBackToCache<IChannel>(cacheKey, CacheBehavior.Default);
            }

            // cache -----------------------------
            SetCacheValue(cacheKey, new CacheApiResponse(channel));
            return channel;
        }

        public async Task<bool> SendMessage(ulong channelId, string content = null, Embed embed = null)
        {
            // request ---------------------------
            try
            {
                if (await _client.GetChannelAsync(channelId) is not ITextChannel channel) return false;

                await channel.SendMessageAsync(content, embed: embed);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to send message to channel '{channelId}'.");
                return false;
            }
            return true;
        }

        public async Task<bool> SendDmMessage(ulong userId, string content)
        {
            IChannel channel = await CreateDmChannel(userId);
            if (channel == null)
            {
                return false;
            }

            return await SendMessage(channel.Id, content, null);
        }

        public async Task<bool> SendDmMessage(ulong userId, Embed embed)
        {
            IChannel channel = await CreateDmChannel(userId);
            if (channel == null)
            {
                return false;
            }

            return await SendMessage(channel.Id, null, embed);
        }

        public async Task ExecuteWebhook(string url, Embed embed = null, string content = null)
        {
            await new DiscordWebhookClient(url).SendMessageAsync(content, embeds: embed != null ? new Embed[1] { embed } : null);
        }

        public Dictionary<string, CacheApiResponse> GetCache()
        {
            return _cache;
        }
        public void RemoveFromCache(CacheKey key)
        {
            if (_cache.ContainsKey(key.GetValue()))
            {
                _cache.Remove(key.GetValue());
            }
        }

        public T GetFromCache<T>(CacheKey key)
        {
            if (_cache.ContainsKey(key.GetValue()))
            {
                return _cache[key.GetValue()].GetContent<T>();
            }
            throw new NotFoundInCacheException();
        }

        public async Task<IApplication> GetCurrentApplicationInfo()
        {
            return await _client.GetApplicationInfoAsync();
        }

        public void AddOrUpdateCache(CacheKey key, CacheApiResponse response)
        {
            _cache[key.GetValue()] = response;
        }
    }
}
