using masz.data;
using masz.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace masz.Services
{
    public class IdentityManager : IIdentityManager
    {
        private Dictionary<string, Identity> identities = new Dictionary<string, Identity>();
        private readonly ILogger<IdentityManager> logger;
        private readonly IDatabase context;
        private readonly IOptions<InternalConfig> config;
        private readonly IDiscordAPIInterface discord;

        public IdentityManager() { }

        public IdentityManager(ILogger<IdentityManager> logger, IOptions<InternalConfig> config, IDiscordAPIInterface discord, IDatabase context)
        {
            this.logger = logger;
            this.config = config;
            this.discord = discord;
            this.context = context;
        }

        private async Task<Identity> RegisterNewIdentity(HttpContext httpContext)
        {
            string key = String.Empty;
            Identity identity = null;
            if (httpContext.Request.Headers.ContainsKey("Authorization")) {
                logger.LogInformation("Registering new TokenIdentity.");
                key = "/api/" + httpContext.Request.Headers["Authorization"];
                string fullToken = httpContext.Request.Headers["Authorization"];
                string token = String.Empty;
                try
                {
                    token = fullToken.Split(' ')[1];  // exclude "Bearer" prefix
                }
                catch (Exception e)
                {
                    logger.LogError("Error while parsing token: " + e.Message);
                }
                identity = new TokenIdentity(token, discord, await this.context.GetAPIToken());
            } else {
                key = httpContext.Request.Cookies["masz_access_token"];
                logger.LogInformation("Registering new DiscordIdentity.");
                string token = await httpContext.GetTokenAsync("Cookies", "access_token");
                identity = new DiscordIdentity(token, discord);
            }
            identities[key] = identity;
            return identity;
        }

        public async Task<Identity> GetIdentity(HttpContext httpContext)
        {
            string key = String.Empty;
            if (httpContext.Request.Headers.ContainsKey("Authorization")) {
                key = "/api/" + httpContext.Request.Headers["Authorization"];
            } else {
                key = httpContext.Request.Cookies["masz_access_token"];
            }
            if (identities.ContainsKey(key))
            {
                Identity identity = identities[key];
                if (identity.ValidUntil >= DateTime.UtcNow)
                {
                    return identity;
                } else
                {
                    identities.Remove(key);
                }
            }

            return await RegisterNewIdentity(httpContext);
        }

        public List<Identity> GetCurrentIdentities()
        {
            return this.identities.Values.ToList();
        }

        public void ClearAllIdentities()
        {
            this.identities.Clear();
        }

        public void ClearOldIdentities()
        {
            this.logger.LogInformation("IdentityManager | Clearing old identities.");
            foreach (var key in this.identities.Keys)
            {
                if (this.identities[key].ValidUntil < DateTime.UtcNow) {
                    this.logger.LogInformation($"IdentityManager | Clearing {key.ToString()}.");
                    this.identities.Remove(key);
                }
            }
            this.logger.LogInformation("IdentityManager | Cleared old identities.");
        }

        public void ClearTokenIdentities()
        {
            this.logger.LogInformation("IdentityManager | Clearing token identities.");
            foreach (var key in this.identities.Keys)
            {
                if (this.identities[key] is TokenIdentity) {
                    this.logger.LogInformation($"IdentityManager | Clearing {key.ToString()}.");
                    this.identities.Remove(key);
                }
            }
            this.logger.LogInformation("IdentityManager | Cleared token identities.");
        }
    }
}
