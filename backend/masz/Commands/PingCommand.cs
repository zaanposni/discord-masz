using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace masz.Commands
{

    public class PingCommand : BaseCommand<PingCommand>
    {
        public PingCommand(IServiceProvider serviceProvider) : base(serviceProvider) { }

        [SlashCommand("ping", "A slash command made to test the DSharpPlusSlashCommands library!")]
        public async Task Ping(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Success!"));
        }
    }
}