using Discord;
using Discord.Interactions;
using MASZ.Attributes;
using MASZ.Enums;

namespace MASZ.Commands
{

    public class WarnModal : BaseCommand<WarnModal>
    {
        [Require(RequireCheckEnum.GuildModerator)]
        [UserCommand("warn")]
        public async Task UserWarn([Summary("user", "user to warn")] IUser user)
        {
            Modal modal = Models.PunishModal.Create(PunishmentType.Warn, $"Warn {user.Username}", user.Id, false);
            await Context.Interaction.RespondWithModalAsync(modal);
        }
    }
}