using System;
using System.Threading.Tasks;
using masz.Models;
using masz.Services;
using Microsoft.AspNetCore.Mvc;

namespace masz.Controllers
{
    public class SimpleController : ControllerBase
    {
        public readonly IDatabase _database;
        private readonly IIdentityManager _identityManager;
        public SimpleController(IServiceProvider serviceProvider)
        {
            _database = (IDatabase) serviceProvider.GetService(typeof(IDatabase));
            _identityManager = (IIdentityManager) serviceProvider.GetService(typeof(IIdentityManager));
        }

        public async Task<Identity> GetIdentity() {
            return await _identityManager.GetIdentity(HttpContext);
        }

        public async Task<GuildConfig> GuildIsRegistered(string guildId) {
            return await _database.SelectSpecificGuildConfig(guildId);
        }
    }
}