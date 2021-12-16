using Discord.Interactions;

namespace MASZ.Commands
{

    public class InviteCommand : BaseCommand<InviteCommand>
    {
        public InviteCommand(IServiceProvider serviceProvider) : base(serviceProvider) { }

        [SlashCommand("invite", "How to invite this bot.")]
        public async Task Invite()
        {
            await Context.Interaction.RespondAsync(Translator.T().CmdInvite());
        }
    }
}