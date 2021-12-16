using Discord;
using Discord.Interactions;
using MASZ.Enums;
using MASZ.Models;
using MASZ.Repositories;

namespace MASZ.Commands
{

    public class PunishmentCommand : BaseCommand<PunishmentCommand>
    {
        public PunishmentCommand(IServiceProvider serviceProvider) : base(serviceProvider) { }

        [SlashCommand("punish", "Punish a user and create a modcase")]
        public async Task Punish(
            [Summary("title", "The title of the modcase")] string title,
            [Summary("user", "User to punish")] IUser user,
            [Summary("punishment", "Choose a punishment")] PunishmentType punishmentType,
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
            await Require(RequireCheckEnum.GuildModerator, RequireCheckEnum.GuildRegistered);

            switch (punishmentType)
            {
                case PunishmentType.Mute:
                    await Require(RequireCheckEnum.GuildStrictModeMute);
                    break;
                case PunishmentType.Kick:
                    await Require(RequireCheckEnum.GuildStrictModeKick);
                    break;
                case PunishmentType.Ban:
                    await Require(RequireCheckEnum.GuildStrictModeBan);
                    break;
            }

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
            modCase.PunishmentType = punishmentType;
            modCase.PunishmentActive = executePunishment;
            modCase.PunishedUntil = DateTime.UtcNow.AddHours(punishedForHours);
            if (punishmentType == PunishmentType.Warn || punishmentType == PunishmentType.Kick || punishedForHours == 0)
            {
                modCase.PunishedUntil = null;  // remove duration for warn and kick
            }
            modCase.CreationType = CaseCreationType.ByCommand;

            ModCase created = await ModCaseRepository.CreateDefault(ServiceProvider, CurrentIdentity).CreateModCase(modCase, executePunishment, sendPublicNotification, sendDmNotification);

            string url = $"{Config.GetBaseUrl()}/guilds/{created.GuildId}/cases/{created.CaseId}";
            await Context.Interaction.RespondAsync(Translator.T().CmdPunish(created.CaseId, url), ephemeral: !sendPublicNotification);
        }
    }
}