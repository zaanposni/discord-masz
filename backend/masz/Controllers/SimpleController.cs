using System;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using masz.Exceptions;
using masz.Models;
using masz.Repositories;
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
        protected readonly IDiscordBot _discordBot;
        protected readonly IScheduler _scheduler;
        protected readonly IServiceProvider _serviceProvider;
        public SimpleController(IServiceProvider serviceProvider)
        {
            _database = (IDatabase) serviceProvider.GetService(typeof(IDatabase));
            _identityManager = (IIdentityManager) serviceProvider.GetService(typeof(IIdentityManager));
            _config = (IOptions<InternalConfig>) serviceProvider.GetService(typeof(IOptions<InternalConfig>));
            _discordAPI = (IDiscordAPIInterface) serviceProvider.GetService(typeof(IDiscordAPIInterface));
            _discordBot = (IDiscordBot) serviceProvider.GetService(typeof(IDiscordBot));
            _scheduler = (IScheduler) serviceProvider.GetService(typeof(IScheduler));
            _serviceProvider = serviceProvider;
        }

        public async Task<Identity> GetIdentity() {
            Identity identity = await _identityManager.GetIdentity(HttpContext);
            if (identity == null) {
                throw new InvalidIdentityException();
            }
            return identity;
        }

        public async Task<DiscordUser> GetCurrentDiscordUser() {
            Identity identity = await GetIdentity();
            return identity.GetCurrentUser();
        }

        public async Task<GuildConfig> GetRegisteredGuild(ulong guildId) {
            try
            {
                return await GuildConfigRepository.CreateDefault(_serviceProvider).GetGuildConfig(guildId);
            } catch (ResourceNotFoundException)
            {
                throw new UnregisteredGuildException(guildId);
            }
        }
    }
}