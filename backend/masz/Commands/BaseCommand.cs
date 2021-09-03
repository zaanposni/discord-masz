using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.SlashCommands;
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

        public BaseCommand(IServiceProvider serviceProvider)
        {
            this._logger = (ILogger<T>) serviceProvider.GetService(typeof(ILogger<T>));
            this._database = (IDatabase) serviceProvider.GetService(typeof(IDatabase));
            this._translator = (ITranslator) serviceProvider.GetService(typeof(ITranslator));
        }

        public override Task<bool> BeforeExecutionAsync(InteractionContext ctx)
        {
            if (ctx.Channel.Type == ChannelType.Text)
            {
                _logger.LogInformation($"{ctx.User.Id} used {ctx.CommandName} in {ctx.Channel.Id} | {ctx.Guild.Name}");
            }
            else
            {
                _logger.LogInformation($"{ctx.User.Id} used {ctx.CommandName} in DM");
            }
            return base.BeforeExecutionAsync(ctx);
        }
    }
}