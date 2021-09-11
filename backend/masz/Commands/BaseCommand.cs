using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using masz.Enums;
using masz.Exceptions;
using masz.Models;
using masz.Repositories;
using masz.Services;
using Microsoft.Extensions.Logging;

namespace masz.Commands
{

    [SlashModuleLifespan(SlashModuleLifespan.Scoped)]
    public class BaseCommand<T> : ApplicationCommandModule
    {

        protected ILogger<T> _logger { get; set; }
        protected IDatabase _database { get; set; }
        protected ITranslator _translator { get; set; }
        protected IIdentityManager _identityManager { get; set; }
        protected Identity _currentIdentity { get; set; }
        protected IDiscordAPIInterface _discordAPI { get; set; }
        protected IServiceProvider _serviceProvider { get; set; }

        public BaseCommand(IServiceProvider serviceProvider)
        {
            _logger = (ILogger<T>) serviceProvider.GetService(typeof(ILogger<T>));
            _database = (IDatabase) serviceProvider.GetService(typeof(IDatabase));
            _translator = (ITranslator) serviceProvider.GetService(typeof(ITranslator));
            _identityManager = (IIdentityManager) serviceProvider.GetService(typeof(IIdentityManager));
            _discordAPI = (IDiscordAPIInterface) serviceProvider.GetService(typeof(IDiscordAPIInterface));
            _serviceProvider = serviceProvider;
        }

        public override async Task<bool> BeforeSlashExecutionAsync(InteractionContext ctx)
        {
            if (ctx.Channel.Type == ChannelType.Text)
            {
                _logger.LogInformation($"{ctx.User.Id} used {ctx.CommandName} in {ctx.Channel.Id} | {ctx.Guild.Name}");
            }
            else
            {
                _logger.LogInformation($"{ctx.User.Id} used {ctx.CommandName} in DM");
            }

            _currentIdentity = await _identityManager.GetIdentity(ctx.User);
            if (_currentIdentity == null)
            {
                _logger.LogError($"Failed to register command identity for '{ctx.User.Id}'.");
                return false;  // do not execute the slash command
            }

            return await base.BeforeSlashExecutionAsync(ctx);
        }

        public async Task<bool> BeforeExecutionAsync(ContextMenuContext ctx)
        {
            if (ctx.Channel.Type == ChannelType.Text)
            {
                _logger.LogInformation($"{ctx.User.Id} used {ctx.CommandName} in {ctx.Channel.Id} | {ctx.Guild.Name}");
            }
            else
            {
                _logger.LogInformation($"{ctx.User.Id} used {ctx.CommandName} in DM");
            }

            _currentIdentity = await _identityManager.GetIdentity(ctx.User);
            if (_currentIdentity == null)
            {
                _logger.LogError($"Failed to register command identity for '{ctx.User.Id}'.");
                return false;  // do not execute the context command
            }

            return await base.BeforeContextMenuExecutionAsync(ctx);
        }

        protected async Task Require(BaseContext ctx, params RequireCheckEnum[] checks)
        {
            foreach (RequireCheckEnum check in checks)
            {
                switch (check)
                {
                    case RequireCheckEnum.GuildRegistered:
                        await RequireRegisteredGuild(ctx);
                        continue;
                    case RequireCheckEnum.GuildMember:
                        await RequireDiscordPermission(ctx, DiscordPermission.Member);
                        continue;
                    case RequireCheckEnum.GuildModerator:
                        await RequireDiscordPermission(ctx, DiscordPermission.Moderator);
                        continue;
                    case RequireCheckEnum.GuildAdmin:
                        await RequireDiscordPermission(ctx, DiscordPermission.Admin);
                        continue;
                    case RequireCheckEnum.GuildMuteRole:
                        await RequireGuildWithMutedRole(ctx);
                        continue;
                    case RequireCheckEnum.SiteAdmin:
                        await RequireSiteAdmin(ctx);
                        continue;
                }
            }
        }

        private Task RequireSiteAdmin(BaseContext ctx)
        {
            if (! _currentIdentity.IsSiteAdmin())
            {
                throw new UnauthorizedException("Only site admins allowed.");
            }
            return Task.CompletedTask;
        }

        private async Task RequireRegisteredGuild(BaseContext ctx)
        {
            try
            {
                await GuildConfigRepository.CreateDefault(_serviceProvider).GetGuildConfig(ctx.Guild.Id);
            } catch (ResourceNotFoundException)
            {
                throw new UnregisteredGuildException(ctx.Guild.Id);
            }
        }

        private async Task RequireGuildWithMutedRole(BaseContext ctx)
        {
            try
            {
                GuildConfig guildConfig = await GuildConfigRepository.CreateDefault(_serviceProvider).GetGuildConfig(ctx.Guild.Id);
                if (guildConfig.MutedRoles.Length == 0)
                {
                    throw new GuildWithoutMutedRoleException(ctx.Guild.Id);
                }
            } catch (ResourceNotFoundException)
            {
                throw new UnregisteredGuildException(ctx.Guild.Id);
            }
        }

        private async Task RequireDiscordPermission(BaseContext ctx, DiscordPermission permission)
        {
            await RequireRegisteredGuild(ctx);
            if (_currentIdentity.IsSiteAdmin())
            {
                return;
            }
            switch(permission)
            {
                case (DiscordPermission.Member):
                    if (_currentIdentity.IsOnGuild(ctx.Guild.Id)) return;
                    break;
                case (DiscordPermission.Moderator):
                    if (await _currentIdentity.HasModRoleOrHigherOnGuild(ctx.Guild.Id)) return;
                    break;
                case (DiscordPermission.Admin):
                    if (await _currentIdentity.HasAdminRoleOnGuild(ctx.Guild.Id)) return;
                    break;
            }
            throw new UnauthorizedException("You are not allowed to do that.");
        }
    }
}