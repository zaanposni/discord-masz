using Discord.Interactions;

namespace MASZ.Commands
{

    public class UrlCommand : BaseCommand<UrlCommand>
    {
        [SlashCommand("url", "Displays the URL MASZ is deployed on.")]
        public async Task Url()
        {
            await Context.Interaction.RespondAsync(Config.GetBaseUrl());
        }
    }
}