using Discord.Interactions;

namespace MASZ.Commands
{

    public class UrlCommand : BaseCommand<UrlCommand>
    {
        public UrlCommand(IServiceProvider serviceProvider) : base(serviceProvider) { }

        [SlashCommand("url", "Displays the URL MASZ is deployed on.")]
        public async Task Url()
        {
            await Context.Interaction.RespondAsync(Config.GetBaseUrl());
        }
    }
}