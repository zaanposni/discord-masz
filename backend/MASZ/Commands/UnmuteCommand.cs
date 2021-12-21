using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using MASZ.Attributes;
using MASZ.Enums;
using MASZ.Extensions;
using MASZ.Models;
using MASZ.Repositories;
using System.Globalization;
using System.Text;

namespace MASZ.Commands
{

    public class UnmuteCommand : BaseCommand<UnmuteCommand>
    {
        [Require(RequireCheckEnum.GuildModerator, RequireCheckEnum.GuildStrictModeMute)]
        [SlashCommand("unmute", "Unmute a user by deactivating all his modcases.")]
        public async Task Unmute([Summary("user", "User to unmute")] IUser user)
        {
            ModCaseRepository repo = ModCaseRepository.CreateDefault(ServiceProvider, CurrentIdentity);
            List<ModCase> modCases = (await repo.GetCasesForGuildAndUser(Context.Guild.Id, user.Id))
                .Where(x => x.PunishmentActive && x.PunishmentType == PunishmentType.Mute).ToList();

            if (modCases.Count == 0)
            {
                await Context.Interaction.RespondAsync(Translator.T().CmdUndoNoCases());
                return;
            }

            StringBuilder interactionString = new();
            interactionString.AppendLine(Translator.T().CmdUndoUnmuteFoundXCases(modCases.Count));
            foreach (ModCase modCase in modCases.Take(5))
            {
                int truncate = 50;
                if (modCase.PunishedUntil != null)
                {
                    truncate = 30;
                }
                interactionString.Append($"- [#{modCase.CaseId} - {modCase.Title.Truncate(truncate)}]");
                interactionString.Append($"({Config.GetBaseUrl()}/guilds/{modCase.GuildId}/cases/{modCase.CaseId})");
                if (modCase.PunishedUntil != null)
                {
                    interactionString.Append($" {Translator.T().Until()} {modCase.PunishedUntil.Value.ToDiscordTS()}");
                }
                interactionString.AppendLine();
            }
            if (modCases.Count > 5)
            {
                interactionString.AppendLine(Translator.T().AndXMore(modCases.Count - 5));
            }

            EmbedBuilder embed = new EmbedBuilder()
                .WithTitle(user.Username)
                .WithDescription(interactionString.ToString())
                .WithColor(Color.Orange);

            embed.AddField(Translator.T().CmdUndoResultTitle(), Translator.T().CmdUndoResultWaiting());

            var button = new ComponentBuilder()
                .WithButton(Translator.T().CmdUndoUnmuteButtonsDelete(), $"unmute-delete:{user.Id}", ButtonStyle.Primary)
                .WithButton(Translator.T().CmdUndoUnmuteButtonsDeactivate(), $"unmute-deactivate:{user.Id}", ButtonStyle.Secondary)
                .WithButton(Translator.T().CmdUndoButtonsCancel(), "unmute-cancel", ButtonStyle.Danger);

            await Context.Interaction.RespondAsync(embed: embed.Build(), components: button.Build());

            IMessage responseMessage = await Context.Interaction.GetOriginalResponseAsync();
        }

        [ComponentInteraction("unmute-delete:*")]
        public async Task Delete(string userID)
        {
            var castInteraction = Context.Interaction as SocketMessageComponent;

            var button = new ComponentBuilder()
                .WithButton(Translator.T().CmdUndoButtonsPublicNotification(), $"unmute-conf-delete:1,{userID}", ButtonStyle.Primary)
                .WithButton(Translator.T().CmdUndoButtonsNoPublicNotification(), $"unmute-conf-delete:0,{userID}", ButtonStyle.Secondary)
                .WithButton(Translator.T().CmdUndoButtonsCancel(), "unmute-cancel", ButtonStyle.Danger);

            var embed = castInteraction.Message.Embeds.FirstOrDefault().ToEmbedBuilder()
                .WithColor(Color.Red);

            embed.Fields = new()
            {
                new EmbedFieldBuilder().WithName(Translator.T().CmdUndoResultTitle()).WithValue(Translator.T().CmdUndoResultWaiting()),
                new EmbedFieldBuilder().WithName(Translator.T().CmdUndoPublicNotificationTitle()).WithValue(Translator.T().CmdUndoPublicNotificationDescription())
            };

            await castInteraction.UpdateAsync(message =>
            {
                message.Embed = embed.Build();
                message.Components = button.Build();
            });
        }

        [ComponentInteraction("unmute-conf-delete:*,*")]
        public async Task DeleteConfirmation(string isPublic, string userID)
        {
            ModCaseRepository repo = ModCaseRepository.CreateDefault(ServiceProvider, CurrentIdentity);
            List<ModCase> modCases = await repo.GetCasesForGuildAndUser(Context.Guild.Id, Convert.ToUInt64(userID));

            modCases = modCases.Where(x => x.PunishmentActive && x.PunishmentType == PunishmentType.Mute).ToList();

            foreach (ModCase modCase in modCases)
            {
                await repo.DeleteModCase(modCase.GuildId, modCase.CaseId, false, true, isPublic == "1");
            }

            var castInteraction = Context.Interaction as SocketMessageComponent;

            var embed = castInteraction.Message.Embeds.FirstOrDefault().ToEmbedBuilder()
                .WithColor(new Color(Convert.ToUInt32(int.Parse("7289da", NumberStyles.HexNumber))));  // discord blurple
            
            embed.Fields = new()
            {
                new EmbedFieldBuilder().WithName(Translator.T().CmdUndoResultTitle()).WithValue(Translator.T().CmdUndoUnmuteResultDeleted())
            };

            await castInteraction.UpdateAsync(message =>
            {
                message.Embed = embed.Build();
                message.Components = new ComponentBuilder().Build();
            });
        }

        [ComponentInteraction("unmute-deactivate:*")]
        public async Task Deactivate(string userID)
        {
            ModCaseRepository repo = ModCaseRepository.CreateDefault(ServiceProvider, CurrentIdentity);
            List<ModCase> modCases = await repo.GetCasesForGuildAndUser(Context.Guild.Id, Convert.ToUInt64(userID));

            modCases = modCases.Where(x => x.PunishmentActive && x.PunishmentType == PunishmentType.Mute).ToList();

            await repo.DeactivateModCase(modCases.ToArray());

            var castInteraction = Context.Interaction as SocketMessageComponent;

            var embed = castInteraction.Message.Embeds.FirstOrDefault().ToEmbedBuilder().WithColor(Color.Green);

            embed.Fields = new()
            {
                new EmbedFieldBuilder().WithName(Translator.T().CmdUndoResultTitle()).WithValue(Translator.T().CmdUndoUnmuteResultDeactivated())
            };

            await castInteraction.UpdateAsync(message =>
            {
                message.Embed = embed.Build();
                message.Components = new ComponentBuilder().Build();
            });
        }

        [ComponentInteraction("unmute-cancel")]
        public async Task Cancel()
        {
            var castInteraction = Context.Interaction as SocketMessageComponent;

            var embed = castInteraction.Message.Embeds.FirstOrDefault().ToEmbedBuilder().WithColor(Color.Red);

            embed.Fields = new()
            {
                new EmbedFieldBuilder().WithName(Translator.T().CmdUndoResultTitle()).WithValue(Translator.T().CmdUndoResultCanceled())
            };

            await castInteraction.UpdateAsync(message =>
            {
                message.Embed = embed.Build();
                message.Components = new ComponentBuilder().Build();
            });
        }
    }
}