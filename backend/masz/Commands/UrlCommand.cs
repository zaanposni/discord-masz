using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace masz.Commands
{

    public class UrlCommand : BaseCommand<UrlCommand>
    {
        public UrlCommand(IServiceProvider serviceProvider) : base(serviceProvider) { }

        [SlashCommand("url", "Displays the URL MASZ is deployed on.")]
        public async Task Url(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"MASZ is deployed on {_config.GetBaseUrl()}"));
        }
    }
}