using Discord.Interactions;

namespace MASZ.Commands
{

    public class GitHubCommand : BaseCommand<GitHubCommand>
    {
        public GitHubCommand(IServiceProvider serviceProvider) : base(serviceProvider) { }

        [SlashCommand("github", "Displays the GitHub repository URL.")]
        public async Task GitHub()
        {
            await Context.Interaction.RespondAsync("https://github.com/zaanposni/discord-MASZ");
        }
    }
}