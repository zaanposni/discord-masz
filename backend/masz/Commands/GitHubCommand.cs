using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace masz.Commands
{

    public class GitHubCommand : BaseCommand<GitHubCommand>
    {
        public GitHubCommand(IServiceProvider serviceProvider) : base(serviceProvider) { }

        [SlashCommand("github", "Displays the GitHub repository URL.")]
        public async Task GitHub(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("https://github.com/zaanposni/discord-masz"));
        }
    }
}