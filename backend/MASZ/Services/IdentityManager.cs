using Discord;
using MASZ.Exceptions;
using MASZ.Models;
using MASZ.Repositories;
using MASZ.Extensions;
using Microsoft.AspNetCore.Authentication;

namespace MASZ.Services
{
    public class IdentityManager
    {
        private readonly Dictionary<string, Identity> identities = new();
        private readonly ILogger<IdentityManager> _logger;
        private readonly DiscordEventHandler _eventHandler;
        private readonly IServiceProvider _serviceProvider;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public IdentityManager() { }

        public IdentityManager(ILogger<IdentityManager> logger, DiscordEventHandler eventHandler, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _eventHandler = eventHandler;
            _serviceProvider = serviceProvider;
            _serviceScopeFactory = serviceScopeFactory;
        }

        private async Task<Identity> RegisterNewIdentity(HttpContext httpContext)
        {
            string key;
            Identity identity;
            if (httpContext.Request.Headers.ContainsKey("Authorization"))
            {
                _logger.LogInformation("Registering new TokenIdentity.");
                key = "/api/" + httpContext.Request.Headers["Authorization"];
                string fullToken = httpContext.Request.Headers["Authorization"];
                string token = string.Empty;
                try
                {
                    token = fullToken.Split(' ')[1];  // exclude "Bearer" prefix
                }
                catch (Exception e)
                {
                    _logger.LogError("Error while parsing token: " + e.Message);
                }
                APIToken registeredToken = null;
                try
                {
                    registeredToken = await TokenRepository.CreateDefault(_serviceProvider).GetToken();
                }
                catch (ResourceNotFoundException) { }
                identity = new TokenIdentity(token, _serviceProvider, registeredToken, _serviceScopeFactory);
            }
            else
            {
                key = httpContext.Request.Cookies["masz_access_token"];
                _logger.LogInformation("Registering new DiscordIdentity.");
                string token = await httpContext.GetTokenAsync("Cookies", "access_token");
                identity = await DiscordOAuthIdentity.Create(token, _serviceProvider, _serviceScopeFactory);
            }
            identities[key] = identity;

            await _eventHandler.OnIdentityRegisteredEvent.InvokeAsync(identity);

            return identity;
        }

        private async Task<Identity> RegisterNewIdentity(IUser user)
        {
            string key = $"/discord/cmd/{user.Id}";
            Identity identity = await DiscordCommandIdentity.Create(user, _serviceProvider, _serviceScopeFactory);
            identities[key] = identity;

            await _eventHandler.OnIdentityRegisteredEvent.InvokeAsync(identity);

            return identity;
        }

        public async Task<Identity> GetIdentity(HttpContext httpContext)
        {
            string key;
            if (httpContext.Request.Headers.ContainsKey("Authorization"))
            {
                key = "/api/" + httpContext.Request.Headers["Authorization"];
            }
            else
            {
                key = httpContext.Request.Cookies["masz_access_token"];
            }
            if (string.IsNullOrEmpty(key))
            {
                throw new UnauthorizedException();
            }
            if (identities.ContainsKey(key))
            {
                Identity identity = identities[key];
                if (identity.ValidUntil >= DateTime.UtcNow)
                {
                    return identity;
                }
                else
                {
                    identities.Remove(key);
                }
            }

            return await RegisterNewIdentity(httpContext);
        }

        public async Task<Identity> GetIdentity(IUser user)
        {
            if (user == null)
            {
                return null;
            }
            string key = $"/discord/cmd/{user.Id}";
            if (identities.ContainsKey(key))
            {
                Identity identity = identities[key];
                if (identity.ValidUntil >= DateTime.UtcNow)
                {
                    return identity;
                }
                else
                {
                    identities.Remove(key);
                }
            }

            return await RegisterNewIdentity(user);
        }

        public List<Identity> GetCurrentIdentities()
        {
            return identities.Values.ToList();
        }

        public void ClearAllIdentities()
        {
            identities.Clear();
        }

        public void ClearOldIdentities()
        {
            _logger.LogInformation("IdentityManager | Clearing old identities.");
            foreach (var key in identities.Keys)
            {
                if (identities[key].ValidUntil < DateTime.UtcNow)
                {
                    _logger.LogInformation($"IdentityManager | Clearing {key}.");
                    identities.Remove(key);
                }
            }
            _logger.LogInformation("IdentityManager | Cleared old identities.");
        }

        public void ClearTokenIdentities()
        {
            _logger.LogInformation("IdentityManager | Clearing token identities.");
            foreach (var key in identities.Keys)
            {
                if (identities[key] is TokenIdentity)
                {
                    _logger.LogInformation($"IdentityManager | Clearing {key}.");
                    identities.Remove(key);
                }
            }
            _logger.LogInformation("IdentityManager | Cleared token identities.");
        }

        public Task<Identity> GetIdentityByUserId(ulong userId)
        {
            throw new NotImplementedException();
        }
    }
}
