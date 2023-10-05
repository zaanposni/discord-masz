using Discord;
using Discord.Interactions;
using MASZ.Attributes;
using MASZ.Enums;

namespace MASZ.Commands
{

    public class MuteModal : BaseCommand<MuteModal>
    {
        [Require(RequireCheckEnum.GuildModerator)]
        [UserCommand("mute")]
        public async Task UserMute([Summary("user", "user to mute")] IUser user)
        {
            Modal modal = Models.PunishModal.Create(PunishmentType.Mute, $"Mute {user.Username}", user.Id, true);
            await Context.Interaction.RespondWithModalAsync(modal);
        }
    }
}