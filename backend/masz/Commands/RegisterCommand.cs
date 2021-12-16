using Discord.Interactions;

namespace MASZ.Commands
{

    public class RegisterCommand : BaseCommand<RegisterCommand>
    {
        public RegisterCommand(IServiceProvider serviceProvider) : base(serviceProvider) { }

        [SlashCommand("register", "Displays the URL to register the current guild.")]
        public async Task Register()
        {
            string url = $"{Config.GetBaseUrl()}/guilds/new";
            if (Context.Guild != null)
            {
                url = $"{Config.GetBaseUrl()}/guilds/new?guildid={Context.Guild.Id}";
            }
            await Context.Interaction.RespondAsync(Translator.T().CmdRegister(url));
        }
    }
}