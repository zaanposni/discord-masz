using Discord.Interactions;

namespace MASZ.Commands
{

    public class GitHubCommand : BaseCommand<GitHubCommand>
    {
        [SlashCommand("github", "Displays the GitHub repository URL.")]
        public async Task GitHub()
        {
            await Context.Interaction.RespondAsync("https://github.com/zaanposni/discord-MASZ");
        }
    }
}