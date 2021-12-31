using Discord;
using Discord.Interactions;
using MASZ.Attributes;
using MASZ.Enums;
using MASZ.Models;
using MASZ.Repositories;

namespace MASZ.Commands
{

    public class MuteCommand : BaseCommand<MuteCommand>
    {
        [Require(RequireCheckEnum.GuildModerator, RequireCheckEnum.GuildRegistered, RequireCheckEnum.GuildStrictModeMute)]
        [SlashCommand("mute", "Mute a user and create a modcase")]
        public async Task Mute(
            [Summary("title", "The title of the modcase")] string title,
            [Summary("user", "User to punish")] IUser user,
                [Choice("None", 0)]
                [Choice("1 Hour", 1)]
                [Choice("1 Day", 24)]
                [Choice("1 Week", 168)]
                [Choice("1 Month", 672)]
            [Summary("hours", "How long the punishment should be")] long punishedForHours = 0,
            [Summary("description", "The description of the modcase")] string description = "",
            [Summary("dm-notification", "Whether to send a dm notification")] bool sendDmNotification = true,
            [Summary("public-notification", "Whether to send a public webhook notification")] bool sendPublicNotification = true,
            [Summary("execute-punishment", "Whether to execute the punishment or just register it.")] bool executePunishment = true)
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
            modCase.PunishmentType = PunishmentType.Mute;
            modCase.PunishmentActive = executePunishment;
            modCase.PunishedUntil = DateTime.UtcNow.AddHours(punishedForHours);
            if (punishedForHours == 0)
            {
                modCase.PunishedUntil = null;  // remove duration for perma mute
            }
            modCase.CreationType = CaseCreationType.ByCommand;

            ModCase created = await ModCaseRepository.CreateDefault(ServiceProvider, CurrentIdentity).CreateModCase(modCase, executePunishment, sendPublicNotification, sendDmNotification);

            string url = $"{Config.GetBaseUrl()}/guilds/{created.GuildId}/cases/{created.CaseId}";
            await Context.Interaction.ModifyOriginalResponseAsync((MessageProperties msg) =>
            {
                msg.Content = Translator.T().CmdPunish(created.CaseId, url);
            }); ;
        }
    }
}