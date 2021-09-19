using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace masz.Commands
{

    public class InviteCommand : BaseCommand<PingCommand>
    {
        public InviteCommand(IServiceProvider serviceProvider) : base(serviceProvider) { }

        [SlashCommand("invite", "How to invite this bot.")]
        public async Task Invite(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("You will have to host your own instance of MASZ on your server or pc.\nCheckout https://github.com/zaanposni/discord-masz#hosting"));
        }
    }
}