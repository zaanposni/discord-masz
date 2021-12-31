using Discord;
using MASZ.Exceptions;
using MASZ.Extensions;
using MASZ.Models;
using MASZ.Repositories;
using Microsoft.AspNetCore.Authentication;

namespace MASZ.Services
{
    public class IdentityManager
    {
        private readonly Dictionary<string, Identity> identities = new();
        private readonly ILogger<IdentityManager> _logger;
        private readonly InternalEventHandler _eventHandler;
        private readonly IServiceProvider _serviceProvider;

        public IdentityManager(ILogger<IdentityManager> logger, InternalEventHandler eventHandler, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _eventHandler = eventHandler;
            _serviceProvider = serviceProvider;
        }

        private static string GetKeyByContext(HttpContext httpContext)
        {
            try
            {
                if (httpContext.Request.Headers.ContainsKey("Authorization"))
                {
                    return "/api/" + httpContext.Request.Headers["Authorization"];
                }
                else
                {
                    return httpContext.Request.Cookies["masz_access_token"];
                }
            }
            catch (KeyNotFoundException)
            {
                throw new UnauthorizedException();
            }
        }

        private async Task<Identity> RegisterNewIdentity(HttpContext httpContext)
        {
            string key = GetKeyByContext(httpContext);
            Identity identity;
            if (httpContext.Request.Headers.ContainsKey("Authorization"))
            {
                _logger.LogInformation("Registering new TokenIdentity.");
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
                identity = new TokenIdentity(token, _serviceProvider, registeredToken);
            }
            else
            {
                _logger.LogInformation("Registering new DiscordIdentity.");
                string token = await httpContext.GetTokenAsync("Cookies", "access_token");
                identity = await DiscordOAuthIdentity.Create(token, _serviceProvider);
            }
            identities[key] = identity;

            _eventHandler.OnIdentityRegisteredEvent.InvokeAsync(identity);

            return identity;
        }

        private async Task<Identity> RegisterNewIdentity(IUser user)
        {
            string key = $"/discord/cmd/{user.Id}";
            Identity identity = await DiscordCommandIdentity.Create(user, _serviceProvider);
            identities[key] = identity;

            _eventHandler.OnIdentityRegisteredEvent.InvokeAsync(identity);

            return identity;
        }

        public void RemoveIdentity(HttpContext httpContext)
        {
            string key = GetKeyByContext(httpContext);
            identities.Remove(key);

            // TODO: onIdentityRemovedEvent.InvokeAsync();
        }

        public async Task<Identity> GetIdentity(HttpContext httpContext)
        {
            string key = GetKeyByContext(httpContext);
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
            int count = 0;
            foreach (var key in identities.Keys)
            {
                if (identities[key].ValidUntil < DateTime.UtcNow)
                {
                    count++;
                    identities.Remove(key);
                }
            }
            _logger.LogInformation($"IdentityManager | Cleared {count} old identities.");
        }

        public void ClearTokenIdentities()
        {
            _logger.LogInformation("IdentityManager | Clearing token identities.");
            int count = 0;
            foreach (var key in identities.Keys)
            {
                if (identities[key] is TokenIdentity)
                {
                    count++;
                    identities.Remove(key);
                }
            }
            _logger.LogInformation($"IdentityManager | Cleared {count} token identities.");
        }

        public Task<Identity> GetIdentityByUserId(ulong userId)
        {
            throw new NotImplementedException();
        }
    }
}
