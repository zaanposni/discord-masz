using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace masz.Commands
{

    public class RegisterCommand : BaseCommand<RegisterCommand>
    {
        public RegisterCommand(IServiceProvider serviceProvider) : base(serviceProvider) { }

        [SlashCommand("register", "Displays the URL to register the current guild.")]
        public async Task Register(InteractionContext ctx)
        {
            string url = $"{_config.GetBaseUrl()}/guilds/new";
            if (ctx.Guild != null)
            {
                url = $"{_config.GetBaseUrl()}/guilds/new?guildid={ctx.Guild.Id}";
            }
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent(_translator.T().CmdRegister(url)));
        }
    }
}