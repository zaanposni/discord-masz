using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace masz.Commands
{

    public class RegisterCommand : BaseCommand<PingCommand>
    {
        public RegisterCommand(IServiceProvider serviceProvider) : base(serviceProvider) { }

        [SlashCommand("register", "Displays the URL to register the current guild.")]
        public async Task Register(InteractionContext ctx)
        {
            var response = new DiscordInteractionResponseBuilder().WithContent($"A siteadmin can register a guild at: {_config.GetBaseUrl()}/guilds/new");
            if (ctx.Guild != null)
            {
                response = response.WithContent($"A siteadmin can register a guild at: {_config.GetBaseUrl()}/guilds/new?guildid={ctx.Guild.Id}");
            }
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, response);
        }
    }
}