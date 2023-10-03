using Discord;
using Discord.Interactions;
using MASZ.Attributes;
using MASZ.Enums;

namespace MASZ.Commands
{

    public class KickModal : BaseCommand<KickModal>
    {
        [Require(RequireCheckEnum.GuildModerator)]
        [UserCommand("kick")]
        public async Task UserKick([Summary("user", "user to kick")] IUser user)
        {
            Modal modal = Models.PunishModal.Create(PunishmentType.Kick, $"Kick {user.Username}", user.Id, false);
            await Context.Interaction.RespondWithModalAsync(modal);
        }
    }
}