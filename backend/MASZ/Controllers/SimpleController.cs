using Discord;
using MASZ.Enums;
using MASZ.Exceptions;
using MASZ.Models;
using MASZ.Repositories;
using MASZ.Services;
using MASZ.Workers;
using Microsoft.AspNetCore.Mvc;

namespace MASZ.Controllers
{
    public class SimpleController : ControllerBase
    {
        protected readonly IdentityManager _identityManager;
        protected readonly InternalConfiguration _config;
        protected readonly DiscordAPIInterface _discordAPI;
        protected readonly DiscordBot _discordBot;
        protected readonly Scheduler _scheduler;
        protected readonly DiscordAnnouncer _discordAnnouncer;
        protected readonly IServiceProvider _serviceProvider;
        protected readonly Translator _translator;

        public SimpleController(IServiceProvider serviceProvider)
        {
            _identityManager = (IdentityManager)serviceProvider.GetRequiredService(typeof(IdentityManager));
            _config = (InternalConfiguration)serviceProvider.GetRequiredService(typeof(InternalConfiguration));
            _discordAPI = (DiscordAPIInterface)serviceProvider.GetRequiredService(typeof(DiscordAPIInterface));
            _discordBot = (DiscordBot)serviceProvider.GetRequiredService(typeof(DiscordBot));
            _scheduler = (Scheduler)serviceProvider.GetRequiredService(typeof(Scheduler));
            _discordAnnouncer = (DiscordAnnouncer)serviceProvider.GetRequiredService(typeof(DiscordAnnouncer));
            _translator = (Translator)serviceProvider.GetRequiredService(typeof(Translator));
            _serviceProvider = serviceProvider;
        }

        public async Task<Identity> GetIdentity()
        {
            Identity identity = await _identityManager.GetIdentity(HttpContext);
            if (identity == null)
            {
                throw new InvalidIdentityException();
            }
            return identity;
        }

        public async Task<IUser> GetCurrentIUser()
        {
            Identity identity = await GetIdentity();
            return identity.GetCurrentUser();
        }

        public async Task<GuildConfig> GetRegisteredGuild(ulong guildId)
        {
            try
            {
                return await GuildConfigRepository.CreateDefault(_serviceProvider).GetGuildConfig(guildId);
            }
            catch (ResourceNotFoundException)
            {
                throw new UnregisteredGuildException(guildId);
            }
        }

        public async Task RequirePermission(ulong guildId, DiscordPermission permission)
        {
            Identity currentIdentity = await GetIdentity();
            if (!await currentIdentity.HasPermissionOnGuild(permission, guildId))
            {
                throw new UnauthorizedException();
            }
        }

        public async Task RequireSiteAdmin()
        {
            Identity currentIdentity = await GetIdentity();
            if (!currentIdentity.IsSiteAdmin())
            {
                throw new UnauthorizedException();
            }
        }
    }
}