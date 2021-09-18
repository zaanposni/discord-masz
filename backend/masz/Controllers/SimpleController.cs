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
        protected readonly IIdentityManager _identityManager;
        protected readonly IInternalConfiguration _config;
        protected readonly IDiscordAPIInterface _discordAPI;
        protected readonly IDiscordBot _discordBot;
        protected readonly IScheduler _scheduler;
        protected readonly IDiscordAnnouncer _discordAnnouncer;
        protected readonly IServiceProvider _serviceProvider;
        protected readonly ITranslator _translator;
        public SimpleController(IServiceProvider serviceProvider)
        {
            _identityManager = (IIdentityManager) serviceProvider.GetService(typeof(IIdentityManager));
            _config = (IInternalConfiguration) serviceProvider.GetService(typeof(IInternalConfiguration));
            _discordAPI = (IDiscordAPIInterface) serviceProvider.GetService(typeof(IDiscordAPIInterface));
            _discordBot = (IDiscordBot) serviceProvider.GetService(typeof(IDiscordBot));
            _scheduler = (IScheduler) serviceProvider.GetService(typeof(IScheduler));
            _discordAnnouncer = (IDiscordAnnouncer) serviceProvider.GetService(typeof(IDiscordAnnouncer));
            _translator = (ITranslator) serviceProvider.GetService(typeof(ITranslator));
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

        public async Task RequirePermission(ulong guildId, DiscordPermission permission)
        {
            GuildConfig guild = await GetRegisteredGuild(guildId);
            Identity currentIdentity = await GetIdentity();
            if(! await currentIdentity.HasPermissionOnGuild(permission, guildId)) {
                throw new UnauthorizedException();
            }
        }

        public async Task RequireSiteAdmin()
        {
            Identity currentIdentity = await GetIdentity();
            if(! currentIdentity.IsSiteAdmin()) {
                throw new UnauthorizedException();
            }
        }
    }
}