using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using MASZ.Attributes;
using MASZ.Enums;
using MASZ.Models;
using MASZ.Repositories;
using MASZ.Utils;

namespace MASZ.Commands
{

    public class PunishModal : BaseModalInteraction<PunishModal>
    {
        public PunishModal(IServiceProvider serviceProvider, IUser user) : base(serviceProvider, user)
        {
        }

        public override RequireCheckEnum[] _checks => new RequireCheckEnum[] { RequireCheckEnum.GuildModerator };

        public override async Task<PreconditionResult> CheckRequirementsAsync(SocketModal modal, IServiceProvider services)
            => await new RequireAttribute(_checks).CheckRequirementsAsync(modal, services);


        public override async Task HandleModal(SocketModal modal)
        {
            if (!modal.GuildId.HasValue) {
                await modal.RespondAsync(Translator.T().CmdViewInvalidGuildId());
                return;
            }

            await modal.DeferAsync(ephemeral: true);

            string punishmentTypeRaw = modal.Data.CustomId.Split(":")[1];
            PunishmentType punishmentType;
            switch (punishmentTypeRaw)
            {
                case "warn":
                    punishmentType = PunishmentType.Warn;
                    break;
                case "mute":
                    punishmentType = PunishmentType.Mute;
                    break;
                case "kick":
                    punishmentType = PunishmentType.Kick;
                    break;
                case "ban":
                    punishmentType = PunishmentType.Ban;
                    break;
                default:
                    punishmentType = PunishmentType.Warn;
                    break;
            }

            ModCase modCase = new()
            {
                Title = modal.Data.Components.First(x => x.CustomId == PunishModalKey.Reason).Value,
                GuildId = modal.GuildId.Value,
                UserId = ulong.Parse(modal.Data.CustomId.Split(":")[2]),
                ModId = CurrentIdentity.GetCurrentUser().Id
            };
            if (string.IsNullOrEmpty(modal.Data.Components.First(x => x.CustomId == PunishModalKey.Description).Value))
            {
                modCase.Description = modCase.Title;
            }
            else
            {
                modCase.Description = modal.Data.Components.First(x => x.CustomId == PunishModalKey.Description).Value;
            }

            var rawDuration = modal.Data.Components.FirstOrDefault(x => x.CustomId == PunishModalKey.Duration);
            if (rawDuration != null && !string.IsNullOrEmpty(rawDuration.Value))
            {
                TimeSpan duration = StringTimeSpan.ParseDateRange(rawDuration.Value);
                modCase.PunishedUntil = DateTime.UtcNow.Add(duration);
            } else {
                modCase.PunishedUntil = null;
            }

            modCase.PunishmentType = punishmentType;
            modCase.PunishmentActive = true;
            modCase.CreationType = CaseCreationType.ByCommand;

            ModCase created = await ModCaseRepository
                .CreateDefault(ServiceProvider, CurrentIdentity)
                .CreateModCase(
                    modCase,
                    true,
                    modal.Data.Components.First(x => x.CustomId == PunishModalKey.PublicNotification).Value == "true",
                    modal.Data.Components.First(x => x.CustomId == PunishModalKey.DmNotification).Value == "true"
                );

            string url = $"{Config.GetBaseUrl()}/guilds/{created.GuildId}/cases/{created.CaseId}";
            await modal.ModifyOriginalResponseAsync((MessageProperties msg) =>
            {
                msg.Content = Translator.T().CmdPunish(created.CaseId, url);
            }); ;
        }
    }
}