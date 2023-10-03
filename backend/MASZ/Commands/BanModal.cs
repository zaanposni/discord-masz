using Discord;
using Discord.Interactions;
using MASZ.Attributes;
using MASZ.Enums;

namespace MASZ.Commands
{

    public class BanModal : BaseCommand<BanModal>
    {
        [Require(RequireCheckEnum.GuildModerator)]
        [UserCommand("ban")]
        public async Task UserBan([Summary("user", "user to ban")] IUser user)
        {
            Modal modal = Models.PunishModal.Create(PunishmentType.Ban, $"Ban {user.Username}", user.Id, true);
            await Context.Interaction.RespondWithModalAsync(modal);
        }
    }
}