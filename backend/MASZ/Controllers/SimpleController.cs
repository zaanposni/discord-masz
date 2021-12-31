using Discord;
using MASZ.Enums;
using MASZ.Exceptions;
using MASZ.Models;
using MASZ.Repositories;
using MASZ.Services;
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
            _identityManager = serviceProvider.GetRequiredService<IdentityManager>();
            _config = serviceProvider.GetRequiredService<InternalConfiguration>();
            _discordAPI = serviceProvider.GetRequiredService<DiscordAPIInterface>();
            _discordBot = serviceProvider.GetRequiredService<DiscordBot>();
            _scheduler = serviceProvider.GetRequiredService<Scheduler>();
            _discordAnnouncer = serviceProvider.GetRequiredService<DiscordAnnouncer>();
            _translator = serviceProvider.GetRequiredService<Translator>();
            _serviceProvider = serviceProvider;
        }

        protected async Task<Identity> GetIdentity()
        {
            Identity identity = await _identityManager.GetIdentity(HttpContext);
            if (identity == null)
            {
                throw new InvalidIdentityException();
            }
            return identity;
        }

        protected async Task<IUser> GetCurrentUser()
        {
            Identity identity = await GetIdentity();
            return identity.GetCurrentUser();
        }

        protected async Task<GuildConfig> GetRegisteredGuild(ulong guildId)
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

        protected async Task RequirePermission(ulong guildId, DiscordPermission permission)
        {
            Identity currentIdentity = await GetIdentity();
            if (!await currentIdentity.HasPermissionOnGuild(permission, guildId))
            {
                throw new UnauthorizedException();
            }
        }

        protected async Task RequireSiteAdmin()
        {
            Identity currentIdentity = await GetIdentity();
            if (!currentIdentity.IsSiteAdmin())
            {
                throw new UnauthorizedException();
            }
        }
    }
}