using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using masz.Enums;
using masz.Extensions;
using masz.Models;
using masz.Repositories;

namespace masz.Commands
{

    public class UnbanCommand : BaseCommand<UnbanCommand>
    {
        public UnbanCommand(IServiceProvider serviceProvider) : base(serviceProvider) { }

        [SlashCommand("unban", "Unban a user by deactivating all his modcases.")]
        public async Task Unban(InteractionContext ctx, [Option("user", "User to unban")] DiscordUser user)
        {
            await Require(ctx, RequireCheckEnum.GuildModerator, RequireCheckEnum.GuildStrictModeBan);

            ModCaseRepository repo = ModCaseRepository.CreateDefault(_serviceProvider, _currentIdentity);
            List<ModCase> modCases = await repo.GetCasesForGuildAndUser(ctx.Guild.Id, user.Id);

            modCases = modCases.Where(x => x.PunishmentActive && x.PunishmentType == PunishmentType.Ban).ToList();

            if (modCases.Count == 0)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent(_translator.T().CmdUndoNoCases()));
                return;
            }

            StringBuilder interactionString = new StringBuilder();
            interactionString.AppendLine(_translator.T().CmdUndoUnbanFoundXCases(modCases.Count));
            foreach (ModCase modCase in modCases.Take(5))
            {
                int truncate = 50;
                if (modCase.PunishedUntil != null)
                {
                    truncate = 30;
                }
                interactionString.Append($"- [#{modCase.CaseId} - {modCase.Title.Truncate(truncate)}]");
                interactionString.Append($"({_config.GetBaseUrl()}/guilds/{modCase.GuildId}/cases/{modCase.CaseId})");
                if (modCase.PunishedUntil != null)
                {
                    interactionString.Append($" {_translator.T().Until()} {modCase.PunishedUntil.Value.ToDiscordTS()}");
                }
                interactionString.AppendLine();
            }
            if (modCases.Count > 5)
            {
                interactionString.AppendLine(_translator.T().AndXMore(modCases.Count -5));
            }

            DiscordEmbedBuilder embed = new DiscordEmbedBuilder()
                .WithTitle(user.Username)
                .WithDescription(interactionString.ToString())
                .WithColor(DiscordColor.Orange);

            embed.AddField(_translator.T().CmdUndoResultTitle(), _translator.T().CmdUndoResultWaiting());

            string uniqueButtonId = "unban_" + Guid.NewGuid().ToString();

            var deleteButton = new DiscordButtonComponent(ButtonStyle.Primary, uniqueButtonId + "_delete", _translator.T().CmdUndoUnmuteButtonsDelete(), false);
            var deactivateButton = new DiscordButtonComponent(ButtonStyle.Success, uniqueButtonId + "_deactivate", _translator.T().CmdUndoUnbanButtonsDeactivate(), false);
            var cancelButton = new DiscordButtonComponent(ButtonStyle.Danger, uniqueButtonId+ "_cancel", _translator.T().CmdUndoButtonsCancel(), false);

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().AddEmbed(embed.Build()).AddComponents(deleteButton, deactivateButton, cancelButton));

            DiscordMessage responseMessage = await ctx.Interaction.GetOriginalResponseAsync();

            InteractivityResult<ComponentInteractionCreateEventArgs> response = await responseMessage.WaitForButtonAsync(
                (ComponentInteractionCreateEventArgs args) => {
                    return args.User.Id == ctx.User.Id && (args.Id.StartsWith(uniqueButtonId));
                });

            embed.ClearFields();
            if (response.TimedOut)
            {
                embed.WithColor(DiscordColor.Red);
                embed.AddField(_translator.T().CmdUndoResultTitle(), _translator.T().CmdUndoResultTimedout());
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed.Build()));
                return;
            }

            if (response.Result.Id == uniqueButtonId + "_delete")
            {
                embed.ClearFields();
                embed.AddField(_translator.T().CmdUndoResultTitle(), _translator.T().CmdUndoResultWaiting());
                embed.AddField(_translator.T().CmdUndoPublicNotificationTitle(), _translator.T().CmdUndoPublicNotificationDescription());

                var publicButton = new DiscordButtonComponent(ButtonStyle.Success, uniqueButtonId + "_n_public", _translator.T().CmdUndoButtonsPublicNotification(), false);
                var privateButton = new DiscordButtonComponent(ButtonStyle.Primary, uniqueButtonId + "_n_private", _translator.T().CmdUndoButtonsNoPublicNotification(), false);
                var cancelNotificationButton = new DiscordButtonComponent(ButtonStyle.Danger, uniqueButtonId+ "_n_cancel", _translator.T().CmdUndoButtonsCancel(), false);

                await response.Result.Interaction.CreateResponseAsync(
                    InteractionResponseType.UpdateMessage,
                    new DiscordInteractionResponseBuilder().AddEmbed(embed.Build()).AddComponents(publicButton, privateButton, cancelNotificationButton));
                InteractivityResult<ComponentInteractionCreateEventArgs> responseNotification = await responseMessage.WaitForButtonAsync(
                    (ComponentInteractionCreateEventArgs args) => {
                        return args.User.Id == ctx.User.Id && (args.Id.StartsWith(uniqueButtonId));
                    });

                embed.ClearFields();
                if (responseNotification.TimedOut)
                {
                    embed.WithColor(DiscordColor.Red);
                    embed.AddField(_translator.T().CmdUndoResultTitle(), _translator.T().CmdUndoResultTimedout());
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed.Build()));
                    return;
                }
                if (!responseNotification.Result.Id.StartsWith(uniqueButtonId + "_n_") || responseNotification.Result.Id.EndsWith("cancel"))
                {
                    embed.WithColor(DiscordColor.Red);
                    embed.AddField(_translator.T().CmdUndoResultTitle(), _translator.T().CmdUndoResultCanceled());
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed.Build()));
                    return;
                }

                bool publicNotification = responseNotification.Result.Id.EndsWith("public");
                foreach (ModCase modCase in modCases)
                {
                    await repo.DeleteModCase(modCase.GuildId, modCase.CaseId, false, true, publicNotification);
                }

                embed.WithColor(new DiscordColor("#7289da"));  // discord blurple
                embed.AddField(_translator.T().CmdUndoResultTitle(), _translator.T().CmdUndoUnbanResultDeleted());
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed.Build()));
                return;
            }
            if (response.Result.Id == uniqueButtonId + "_deactivate")
            {
                await repo.DeactivateModCase(modCases.ToArray());

                embed.WithColor(DiscordColor.Green);
                embed.AddField(_translator.T().CmdUndoResultTitle(), _translator.T().CmdUndoUnbanResultDeactivated());
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed.Build()));
                return;
            }
            embed.WithColor(DiscordColor.Red);
            embed.AddField(_translator.T().CmdUndoResultTitle(), _translator.T().CmdUndoResultCanceled());
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed.Build()));
        }
    }
}