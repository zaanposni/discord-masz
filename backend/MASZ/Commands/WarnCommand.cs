using Discord;
using Discord.Interactions;
using MASZ.Attributes;
using MASZ.Enums;
using MASZ.Models;
using MASZ.Repositories;

namespace MASZ.Commands
{

    public class WarnCommand : BaseCommand<WarnCommand>
    {
        [Require(RequireCheckEnum.GuildModerator, RequireCheckEnum.GuildRegistered)]
        [SlashCommand("warn", "Warn a user and create a modcase")]
        public async Task Warn(
            [Summary("title", "The title of the modcase")] string title,
            [Summary("user", "User to punish")] IUser user,
            [Summary("description", "The description of the modcase")] string description = "",
            [Summary("dm-notification", "Whether to send a dm notification")] bool sendDmNotification = true,
            [Summary("public-notification", "Whether to send a public webhook notification")] bool sendPublicNotification = true)
        {
            await Context.Interaction.DeferAsync(ephemeral: !sendPublicNotification);
            ModCase modCase = new()
            {
                Title = title,
                GuildId = Context.Guild.Id,
                UserId = user.Id,
                ModId = CurrentIdentity.GetCurrentUser().Id
            };
            if (string.IsNullOrEmpty(description))
            {
                modCase.Description = title;
            }
            else
            {
                modCase.Description = description;
            }
            modCase.PunishmentType = PunishmentType.Warn;
            modCase.PunishmentActive = true;
            modCase.PunishedUntil = null;
            modCase.CreationType = CaseCreationType.ByCommand;

            ModCase created = await ModCaseRepository.CreateDefault(ServiceProvider, CurrentIdentity).CreateModCase(modCase, true, sendPublicNotification, sendDmNotification);

            string url = $"{Config.GetBaseUrl()}/guilds/{created.GuildId}/cases/{created.CaseId}";
            await Context.Interaction.ModifyOriginalResponseAsync((MessageProperties msg) =>
            {
                msg.Content = Translator.T().CmdPunish(created.CaseId, url);
            }); ;
        }
    }
}