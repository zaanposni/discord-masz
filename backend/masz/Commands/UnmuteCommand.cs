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
using Microsoft.Extensions.Logging;

namespace masz.Commands
{

    public class UnmuteCommand : BaseCommand<UnmuteCommand>
    {
        public UnmuteCommand(IServiceProvider serviceProvider) : base(serviceProvider) { }

        [SlashCommand("unmute", "Unmute a user by deactivating all his modcases.")]
        public async Task Unmute(InteractionContext ctx, [Option("user", "User to unmute")] DiscordUser user)
        {
            await Require(ctx, RequireCheckEnum.GuildModerator);

            ModCaseRepository repo = ModCaseRepository.CreateDefault(_serviceProvider, _currentIdentity);
            List<ModCase> modCases = await repo.GetCasesForGuildAndUser(ctx.Guild.Id, user.Id);

            modCases = modCases.Where(x => x.PunishmentActive && x.PunishmentType == PunishmentType.Mute).ToList();

            if (modCases.Count == 0)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent(_translator.T().CmdUnmuteNoCases()));
                return;
            }

            if (modCases.Count == 1)
            {
                await repo.DeactivateModCase(modCases[0]);
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent(_translator.T().CmdUnmuteUnmuteWith1Case(modCases[0].CaseId)));
                return;
            }

            StringBuilder interactionString = new StringBuilder();
            interactionString.AppendLine(_translator.T().CmdUnmuteFoundXCases(modCases.Count));
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

            embed.AddField(_translator.T().CmdUnmuteResult(), _translator.T().CmdUnmuteWaiting());

            string uniqueButtonId = "unmute_" + Guid.NewGuid().ToString();

            var unmuteButton = new DiscordButtonComponent(ButtonStyle.Success, uniqueButtonId + "_unmute", _translator.T().CmdUnmuteUnmute(), false);
            var cancelButton = new DiscordButtonComponent(ButtonStyle.Danger, uniqueButtonId+ "_cancel", _translator.T().CmdUnmuteCancel(), false);

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().AddEmbed(embed.Build()).AddComponents(unmuteButton, cancelButton));

            DiscordMessage responseMessage = await ctx.Interaction.GetOriginalResponseAsync();

            InteractivityResult<ComponentInteractionCreateEventArgs> response = await responseMessage.WaitForButtonAsync(
                (ComponentInteractionCreateEventArgs args) => {
                    return args.User.Id == ctx.User.Id && (args.Id.StartsWith(uniqueButtonId));
                });

            embed.ClearFields();
            if (response.TimedOut)
            {
                embed.WithColor(DiscordColor.Red);
                embed.AddField(_translator.T().CmdUnmuteResult(), _translator.T().CmdUnmuteTimedout());
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed.Build()));
                return;
            }

            if (response.Result.Id == uniqueButtonId + "_unmute")
            {
                await repo.DeactivateModCase(modCases.ToArray());

                embed.WithColor(DiscordColor.Green);
                embed.AddField(_translator.T().CmdUnmuteResult(), _translator.T().CmdUnmuteDone());
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed.Build()));
                return;
            }
            embed.WithColor(DiscordColor.Red);
            embed.AddField(_translator.T().CmdUnmuteResult(), _translator.T().CmdUnmuteCanceled());
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed.Build()));
        }
    }
}