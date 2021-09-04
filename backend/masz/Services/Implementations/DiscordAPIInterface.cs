using DSharpPlus;
using DSharpPlus.Entities;
using masz.Exceptions;
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
        private readonly ILogger<DiscordAPIInterface> _logger;
        private readonly IOptions<InternalConfig> _config;
        private readonly IDiscordBot _discordBot;
        private readonly DiscordRestClient _discordRestClient;
        private Dictionary<string, CacheApiResponse> _cache = new Dictionary<string, CacheApiResponse>();

        public DiscordAPIInterface() {  }
        public DiscordAPIInterface(ILogger<DiscordAPIInterface> logger, IOptions<InternalConfig> config, IDiscordBot discordBot)
        {
            this._logger = logger;
            this._config = config;
            this._discordBot = discordBot;
            this._discordRestClient = new DiscordRestClient(new DiscordConfiguration
            {
                Token = _config.Value.DiscordBotToken,
                TokenType = TokenType.Bot,
            });
        }

        public DiscordRestClient GetOAuthClient(string token)
        {
            return new DiscordRestClient(new DiscordConfiguration
            {
                Token = token,
                TokenType = TokenType.Bearer,
            });
        }

        private T TryGetFromCache<T>(string cacheKey, CacheBehavior cacheBehavior)
        {
            if (cacheBehavior == CacheBehavior.OnlyCache) {
                if (_cache.ContainsKey(cacheKey)) {
                    return _cache[cacheKey].GetContent<T>();
                } else {
                    throw new NotFoundInCacheException(cacheKey);
                }
            }
            if (_cache.ContainsKey(cacheKey) && cacheBehavior == CacheBehavior.Default) {
                if (! _cache[cacheKey].IsExpired()) {
                    return _cache[cacheKey].GetContent<T>();
                }
                _cache.Remove(cacheKey);
            }
            return default(T);
        }

        private T FallBackToCache<T>(string cacheKey, CacheBehavior cacheBehavior)
        {
            if (cacheBehavior != CacheBehavior.IgnoreCache)
            {
                if (_cache.ContainsKey(cacheKey))
                {
                    if (! _cache[cacheKey].IsExpired())
                    {
                        return _cache[cacheKey].GetContent<T>();
                    }
                    _cache.Remove(cacheKey);
                }
            }
            return default(T);
        }

        public async Task<List<DiscordBan>> GetGuildBans(ulong guildId, CacheBehavior cacheBehavior)
        {
            // do cache stuff --------------------
            string cacheKey = $"/guilds/{guildId}/bans";
            List<DiscordBan> bans = null;
            try
            {
                bans = TryGetFromCache<List<DiscordBan>>(cacheKey, cacheBehavior);
                if (bans != null) return bans;
            } catch (NotFoundInCacheException)
            {
                return new List<DiscordBan>();
            }

            // rqeuest ---------------------------
            try
            {
                bans = (await _discordRestClient.GetGuildBansAsync(guildId)).ToList();
            } catch (Exception e)
            {
                _logger.LogError($"Failed to fetch guild bans for guild '{guildId}' from API.", e);
                return FallBackToCache<List<DiscordBan>>(cacheKey, cacheBehavior);
            }

            // cache -----------------------------
            _cache[cacheKey] = new CacheApiResponse(bans);
            foreach (DiscordBan ban in bans)
            {
                _cache[$"{cacheKey}/{ban.User.Id}"] = new CacheApiResponse(ban.User);
                _cache[$"/users/{ban.User.Id}"] = new CacheApiResponse(ban.User);
            }
            return bans;
        }

        public async Task<DiscordBan> GetGuildUserBan(ulong guildId, ulong userId, CacheBehavior cacheBehavior)
        {
            // do cache stuff --------------------
            string cacheKey = $"/guilds/{guildId}/bans/{userId}";
            DiscordBan ban = null;
            try
            {
                ban = TryGetFromCache<DiscordBan>(cacheKey, cacheBehavior);
                if (ban != null) return ban;
            } catch (NotFoundInCacheException)
            {
                return ban;
            }

            // rqeuest ---------------------------
            try
            {
                ban = await  _discordRestClient.GetGuildBanAsync(guildId, userId);
            } catch (Exception e)
            {
                _logger.LogError($"Failed to fetch guild ban for guild '{guildId}' and user '{userId}' from API.", e);
                return FallBackToCache<DiscordBan>(cacheKey, cacheBehavior);
            }

            // cache -----------------------------
            _cache[cacheKey] = new CacheApiResponse(ban);
            _cache[$"/users/{ban.User.Id}"] = new CacheApiResponse(ban.User);
            return ban;
        }

        public async Task<DiscordUser> FetchUserInfo(ulong userId, CacheBehavior cacheBehavior)
        {
            // do cache stuff --------------------
            string cacheKey = $"/users/{userId}";
            DiscordUser user = null;
            try
            {
                user = TryGetFromCache<DiscordUser>(cacheKey, cacheBehavior);
                if (user != null) return user;
            } catch (NotFoundInCacheException)
            {
                return user;
            }

            // rqeuest ---------------------------
            try
            {
                user = await _discordRestClient.GetUserAsync(userId);
            } catch (Exception e)
            {
                _logger.LogError($"Failed to fetch user '{userId}' from API.", e);
                return FallBackToCache<DiscordUser>(cacheKey, cacheBehavior);
            }

            // cache -----------------------------
            _cache[cacheKey] = new CacheApiResponse(user);
            return user;
        }

        public async Task<List<DiscordMember>> FetchGuildMembers(ulong guildId, CacheBehavior cacheBehavior)
        {
            // do cache stuff --------------------
            string cacheKey = $"/guilds/{guildId}/members";
            List<DiscordMember> members = null;
            try
            {
                members = TryGetFromCache<List<DiscordMember>>(cacheKey, cacheBehavior);
                if (members != null) return members;
            } catch (NotFoundInCacheException)
            {
                return new List<DiscordMember>();
            }

            // rqeuest ---------------------------
            try
            {
                DiscordGuild guild = await FetchGuildInfo(guildId, cacheBehavior);
                if (guild == null)
                {
                    return new List<DiscordMember>();
                }
                members = (await guild.GetAllMembersAsync()).ToList();
            } catch (Exception e)
            {
                _logger.LogError($"Failed to fetch members for guild '{guildId}' from API.", e);
                return FallBackToCache<List<DiscordMember>>(cacheKey, cacheBehavior);
            }

            // cache -----------------------------
            foreach (DiscordMember item in members)
            {
                _cache[$"/guilds/{guildId}/members/{item.Id}"] = new CacheApiResponse(item);
                _cache[$"/users/{item.Id}"] = new CacheApiResponse((DiscordUser) item);
            }
            _cache[cacheKey] = new CacheApiResponse(members);
            return members;
        }

        public async Task<DiscordUser> FetchCurrentUserInfo(string token, CacheBehavior cacheBehavior)
        {
            // do cache stuff --------------------
            string cacheKey = $"/users/{token}";
            DiscordUser user = null;
            try
            {
                user = TryGetFromCache<DiscordUser>(cacheKey, cacheBehavior);
                if (user != null) return user;
            } catch (NotFoundInCacheException)
            {
                return user;
            }

            // rqeuest ---------------------------
            try
            {
                user = await GetOAuthClient(token).GetCurrentUserAsync();
            } catch (Exception e)
            {
                _logger.LogError($"Failed to fetch current user for token '{token}' from API.", e);
                return FallBackToCache<DiscordUser>(cacheKey, cacheBehavior);
            }

            // cache -----------------------------
            _cache[cacheKey] = new CacheApiResponse(user);
            return user;
        }

        public DiscordUser GetCurrentBotInfo(CacheBehavior cacheBehavior)
        {
            return _discordRestClient.CurrentUser;
        }

        public async Task<List<DiscordChannel>> FetchGuildChannels(ulong guildId, CacheBehavior cacheBehavior)
        {
            // do cache stuff --------------------
            string cacheKey = $"/guilds/{guildId}/channels";
            List<DiscordChannel> channels = null;
            try
            {
                channels = TryGetFromCache<List<DiscordChannel>>(cacheKey, cacheBehavior);
                if (channels != null) return channels;
            } catch (NotFoundInCacheException)
            {
                return new List<DiscordChannel>();
            }

            // rqeuest ---------------------------
            try
            {
                channels = (await _discordRestClient.GetGuildChannelsAsync(guildId)).ToList();
            } catch (Exception e)
            {
                _logger.LogError($"Failed to fetch guild channels for guild '{guildId}' from API.", e);
                return FallBackToCache<List<DiscordChannel>>(cacheKey, cacheBehavior);
            }

            // cache -----------------------------
            _cache[cacheKey] = new CacheApiResponse(channels);
            return channels;
        }

        public async Task<DiscordGuild> FetchGuildInfo(ulong guildId, CacheBehavior cacheBehavior)
        {
            // do cache stuff --------------------
            string cacheKey = $"/guilds/{guildId}";
            DiscordGuild guild = null;
            try
            {
                guild = TryGetFromCache<DiscordGuild>(cacheKey, cacheBehavior);
                if (guild != null) return guild;
            } catch (NotFoundInCacheException)
            {
                return null;
            }

            // rqeuest ---------------------------
            try
            {
                guild = await _discordRestClient.GetGuildAsync(guildId);
            } catch (Exception e)
            {
                _logger.LogError($"Failed to fetch guild '{guildId}' from API.", e);
                return FallBackToCache<DiscordGuild>(cacheKey, cacheBehavior);
            }

            // cache -----------------------------
            _cache[cacheKey] = new CacheApiResponse(guild);
            return guild;
        }

        public async Task<List<DiscordGuild>> FetchGuildsOfCurrentUser(string token, CacheBehavior cacheBehavior)
        {
            // do cache stuff --------------------
            string cacheKey = $"/users/{token}/guilds";
            List<DiscordGuild> guilds = null;
            try
            {
                guilds = TryGetFromCache<List<DiscordGuild>>(cacheKey, cacheBehavior);
                if (guilds != null) return guilds;
            } catch (NotFoundInCacheException)
            {
                return new List<DiscordGuild>();
            }

            // rqeuest ---------------------------
            try
            {
                guilds = (await GetOAuthClient(token).GetCurrentUserGuildsAsync(limit: 200)).ToList();  // max 200 guilds
            } catch (Exception e)
            {
                _logger.LogError($"Failed to fetch guilds of current user for token '{token}' from API.", e);
                return FallBackToCache<List<DiscordGuild>>(cacheKey, cacheBehavior);
            }

            // cache -----------------------------
            _cache[cacheKey] = new CacheApiResponse(guilds);
            return guilds;
        }

        public async Task<DiscordMember> FetchMemberInfo(ulong guildId, ulong userId, CacheBehavior cacheBehavior)
        {
            // do cache stuff --------------------
            string cacheKey = $"/guilds/{guildId}/members/{userId}";
            DiscordMember member = null;
            try
            {
                member = TryGetFromCache<DiscordMember>(cacheKey, cacheBehavior);
                if (member != null) return member;
            } catch (NotFoundInCacheException)
            {
                return null;
            }

            // rqeuest ---------------------------
            try
            {
                member = await _discordRestClient.GetGuildMemberAsync(guildId, userId);
            } catch (Exception e)
            {
                _logger.LogError($"Failed to fetch guild '{guildId}' member '{userId}' from API.", e);
                return FallBackToCache<DiscordMember>(cacheKey, cacheBehavior);
            }

            // cache -----------------------------
            _cache[cacheKey] = new CacheApiResponse(member);
            _cache[$"/users/{member.Id}"] = new CacheApiResponse((DiscordUser) member);
            return member;
        }

        public Task<DiscordMessage> GetDiscordMessage(ulong channelId, ulong messageId, CacheBehavior cacheBehavior)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> BanUser(ulong guildId, ulong userId)
        {
            // rqeuest ---------------------------
            try
            {
                DiscordGuild guild = await FetchGuildInfo(guildId, CacheBehavior.Default);
                if (guild == null) return false;
                await guild.BanMemberAsync(userId, 0);
            } catch (Exception e)
            {
                _logger.LogError($"Failed to ban user '{userId}' from guild '{guildId}'.", e);
                return false;
            }
            return true;
        }

        public async Task<bool> UnBanUser(ulong guildId, ulong userId)
        {
            // rqeuest ---------------------------
            try
            {
                DiscordGuild guild = await FetchGuildInfo(guildId, CacheBehavior.Default);
                if (guild == null) return false;
                await guild.UnbanMemberAsync(userId);
            } catch (Exception e)
            {
                _logger.LogError($"Failed to unban user '{userId}' from guild '{guildId}'.", e);
                return false;
            }
            return true;
        }

        public async Task<bool> GrantGuildUserRole(ulong guildId, ulong userId, ulong roleId)
        {
            // rqeuest ---------------------------
            try
            {
                DiscordGuild guild = await FetchGuildInfo(guildId, CacheBehavior.Default);
                if (guild == null) return false;
                DiscordMember member = await FetchMemberInfo(guildId, userId, CacheBehavior.Default);
                if (member == null) return false;
                if(! guild.Roles.ContainsKey(roleId)) return false;
                await member.GrantRoleAsync(guild.Roles[roleId]);
            } catch (Exception e)
            {
                _logger.LogError($"Failed to grant user '{userId}' from guild '{guildId}' role '{roleId}'.", e);
                return false;
            }
            return true;
        }

        public async Task<bool> RemoveGuildUserRole(ulong guildId, ulong userId, ulong roleId)
        {
            // rqeuest ---------------------------
            try
            {
                DiscordGuild guild = await FetchGuildInfo(guildId, CacheBehavior.Default);
                if (guild == null) return false;
                DiscordMember member = await FetchMemberInfo(guildId, userId, CacheBehavior.Default);
                if (member == null) return false;
                if(! guild.Roles.ContainsKey(roleId)) return false;
                await member.RevokeRoleAsync(guild.Roles[roleId]);
            } catch (Exception e)
            {
                _logger.LogError($"Failed to revoke user '{userId}' from guild '{guildId}' role '{roleId}'.", e);
                return false;
            }
            return true;
        }

        public async Task<bool> KickGuildUser(ulong guildId, ulong userId)
        {
            // rqeuest ---------------------------
            try
            {
                DiscordMember member = await FetchMemberInfo(guildId, userId, CacheBehavior.Default);
                if (member == null) return false;
                await member.RemoveAsync();
            } catch (Exception e)
            {
                _logger.LogError($"Failed to kick user '{userId}' from guild '{guildId}'.", e);
                return false;
            }
            return true;
        }

        public async Task<DiscordChannel> CreateDmChannel(ulong userId)
        {
            // do cache stuff --------------------
            string cacheKey = $"/users/@me/channels/{userId}";
            DiscordChannel channel = null;
            try
            {
                channel = TryGetFromCache<DiscordChannel>(cacheKey, CacheBehavior.Default);
                if (channel != null) return channel;
            } catch (NotFoundInCacheException)
            {
                return null;
            }

            // rqeuest ---------------------------
            try
            {
                channel = await _discordRestClient.CreateDmAsync(userId);
            } catch (Exception e)
            {
                _logger.LogError($"Failed to create dm with user '{userId}'.", e);
                return FallBackToCache<DiscordChannel>(cacheKey, CacheBehavior.Default);
            }

            // cache -----------------------------
            _cache[cacheKey] = new CacheApiResponse(channel);
            return channel;
        }

        public async Task<bool> SendMessage(ulong channelId, string content = null, DiscordEmbed embed = null)
        {
            // rqeuest ---------------------------
            try
            {
                DiscordChannel channel = await  _discordRestClient.GetChannelAsync(channelId);
                if (channel == null) return false;
                await channel.SendMessageAsync(content, embed);
            } catch (Exception e)
            {
                _logger.LogError($"Failed to send message to channel '{channelId}'.", e);
                return false;
            }
            return true;
        }

        public async Task<bool> SendDmMessage(ulong userId, string content)
        {
            DiscordChannel channel = await CreateDmChannel(userId);
            if (channel == null) {
                return false;
            }

            return await SendMessage(channel.Id, content);
        }

        public async Task<bool> ExecuteWebhook(string url, DiscordEmbed embed = null, string content = null)
        {
            DiscordWebhookBuilder builder = new DiscordWebhookBuilder();
            if (embed != null)
            {
                builder.AddEmbed(embed);
            }
            if (content != null)
            {
                builder.WithContent(content);
            }
            ulong id;
            string token;
            try {
                var splitted = url.Split('/');
                id = ulong.Parse(splitted[splitted.Length - 2]);
                token = splitted[splitted.Length - 1];
            } catch (Exception e)
            {
                _logger.LogError($"Failed to parse webhook url '{url}'.", e);
                return false;
            }
            return (await _discordRestClient.ExecuteWebhookAsync(id, token, builder)) != null;
        }

        public Dictionary<string, CacheApiResponse> GetCache()
        {
            return _cache;
        }

    }
}
