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

        private async Task RegisterNewIdentity(HttpContext httpContext)
        {
            logger.LogInformation("Registering new Identity.");
            string key = httpContext.Request.Cookies["masz_access_token"];
            string token = await httpContext.GetTokenAsync("access_token");
            if (identities.ContainsKey(key))
            {
                identities.Remove(key);
            }
            Identity identity = new Identity(token, discord);
            identities.Add(key, identity);
            logger.LogInformation("New identity registered.");
        }

        public async Task<Identity> GetIdentity(HttpContext httpContext)
        {
            logger.LogInformation("Providing identity.");
            string key = httpContext.Request.Cookies["masz_access_token"];
            string token = await httpContext.GetTokenAsync("access_token");
            if (identities.ContainsKey(key))
            {
                Identity identity = identities[key];
                if (identity.ValidUntil >= DateTime.Now)
                {
                    logger.LogInformation("Returning identity.");
                    return identity;
                }
            }

            logger.LogInformation("Identity not registered yet or invalid. Creating new one.");
            await RegisterNewIdentity(httpContext);
            if (identities.ContainsKey(key))
            {
                Identity identity = identities[key];
                if (identity.ValidUntil >= DateTime.Now)
                {
                    logger.LogInformation("Returning identity.");
                    return identity;
                }
            }

            logger.LogInformation("Identity is still null or invalid. Returning null.");
            return null;
        }
    }
}
