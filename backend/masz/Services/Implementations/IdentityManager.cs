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
            logger.LogInformation("Registering new Identity.");
            string key = httpContext.Request.Cookies["masz_access_token"];
            string token = await httpContext.GetTokenAsync("access_token");
            Identity identity = new Identity(token, discord);
            identities[key] = identity;
            return identity;
        }

        public async Task<Identity> GetIdentity(HttpContext httpContext)
        {
            string key = httpContext.Request.Cookies["masz_access_token"];
            string token = await httpContext.GetTokenAsync("access_token");
            if (identities.ContainsKey(key))
            {
                Identity identity = identities[key];
                if (identity.ValidUntil >= DateTime.Now)
                {
                    return identity;
                }
            }

            return await RegisterNewIdentity(httpContext);
        }

        public List<Identity> GetCurrentIdentities()
        {
            return this.identities.Values.ToList();
        }
    }
}
