using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using masz.Exceptions;
using masz.Models;
using masz.Services;
using Microsoft.Extensions.Logging;

namespace masz.Commands
{

    [SlashModuleLifespan(SlashModuleLifespan.Scoped)]
    public class BaseCommand<T> : SlashCommandModule
    {

        protected ILogger<T> _logger { get; set; }
        protected IDatabase _database { get; set; }
        protected ITranslator _translator { get; set; }
        protected IIdentityManager _identityManager { get; set; }
        protected Identity _currentIdentity { get; set; }

        public BaseCommand(IServiceProvider serviceProvider)
        {
            this._logger = (ILogger<T>) serviceProvider.GetService(typeof(ILogger<T>));
            this._database = (IDatabase) serviceProvider.GetService(typeof(IDatabase));
            this._translator = (ITranslator) serviceProvider.GetService(typeof(ITranslator));
            this._identityManager = (IIdentityManager) serviceProvider.GetService(typeof(IIdentityManager));
        }

        public override async Task<bool> BeforeExecutionAsync(InteractionContext ctx)
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

            return await base.BeforeExecutionAsync(ctx);
        }

        protected async Task RequireRegisteredGuild(InteractionContext ctx)
        {
            if (await _database.SelectSpecificGuildConfig(ctx.Guild.Id) == null)
            {
                throw new UnregisteredGuildException(ctx.Guild.Id);
            }
        }

        protected async Task RequireDiscordPermission(InteractionContext ctx, DiscordPermission permission)
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