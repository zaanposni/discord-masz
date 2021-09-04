using System;
using System.Threading.Tasks;
using masz.Models;
using masz.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace masz.Controllers
{
    public class SimpleController : ControllerBase
    {
        protected readonly IDatabase _database;
        protected readonly IIdentityManager _identityManager;
        protected readonly IOptions<InternalConfig> _config;
        protected readonly IDiscordAPIInterface _discordAPI;
        public SimpleController(IServiceProvider serviceProvider)
        {
            _database = (IDatabase) serviceProvider.GetService(typeof(IDatabase));
            _identityManager = (IIdentityManager) serviceProvider.GetService(typeof(IIdentityManager));
            _config = (IOptions<InternalConfig>) serviceProvider.GetService(typeof(IOptions<InternalConfig>));
            _discordAPI = (IDiscordAPIInterface) serviceProvider.GetService(typeof(IDiscordAPIInterface));
        }

        public async Task<Identity> GetIdentity() {
            return await _identityManager.GetIdentity(HttpContext);
        }

        public async Task<GuildConfig> GuildIsRegistered(ulong guildId) {
            return await _database.SelectSpecificGuildConfig(guildId);
        }
    }
}