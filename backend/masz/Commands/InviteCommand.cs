using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace masz.Commands
{

    public class InviteCommand : BaseCommand<InviteCommand>
    {
        public InviteCommand(IServiceProvider serviceProvider) : base(serviceProvider) { }

        [SlashCommand("invite", "How to invite this bot.")]
        public async Task Invite(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent(_translator.T().CmdInvite()));
        }
    }
}