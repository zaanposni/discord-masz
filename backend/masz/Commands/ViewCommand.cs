using System;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using masz.Enums;
using masz.Exceptions;
using masz.Models;
using masz.Repositories;
using masz.Services;
using Microsoft.Extensions.DependencyInjection;

namespace masz.Commands
{

    public class ViewCommand : BaseCommand<ViewCommand>
    {
        private readonly string SCALES_EMOTE = "\u2696";
        private readonly string SCROLL_EMOTE = "\uD83C\uDFF7";
        private readonly string ALARM_CLOCK = "\u23F0";
        public ViewCommand(IServiceProvider serviceProvider) : base(serviceProvider) { }

        [SlashCommand("view", "View details of a modcase.")]
        public async Task View(InteractionContext ctx, [Option("id", "the id of the case")] long caseId, [Option("guildid", "the id of the guild")] string guildId = "")
        {
            // parse to ulong because discord sux
            ulong parsedGuildId = 0;
            if (ctx.Guild == null)
            {
                if (! ulong.TryParse(guildId, out parsedGuildId))
                {
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Please specify a valid guildid."));
                    return;
                }
            } else if (String.IsNullOrEmpty(guildId))
            {
                parsedGuildId = (ulong) ctx.Guild.Id;
            } else
            {
                try
                {
                    parsedGuildId = ulong.Parse(guildId);
                } catch (Exception)
                {
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Please specify a valid guildid."));
                    return;
                }
            }
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

            ModCase modCase;
            try
            {
                modCase = await ModCaseRepository.CreateDefault(_serviceProvider, _currentIdentity).GetModCase(parsedGuildId, (int) caseId);
            } catch (ResourceNotFoundException)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Not found."));
                return;
            }

            if (! await _currentIdentity.IsAllowedTo(APIActionPermission.View, modCase))
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"You are not allowed to view this case."));
                return;
            }

            DiscordEmbedBuilder embed = new DiscordEmbedBuilder();
            embed.WithUrl($"{_config.GetBaseUrl()}/guilds/{modCase.GuildId}/cases/{modCase.CaseId}");
            embed.WithTimestamp(modCase.CreatedAt);

            DiscordUser suspect = await _discordAPI.FetchUserInfo(modCase.UserId, CacheBehavior.Default);
            if (suspect != null)
            {
                embed.WithThumbnail(suspect.AvatarUrl);
            }

            string modCaseSubStringTitle = modCase.Title.Substring(0, Math.Min(modCase.Title.Length, 200));
            if (modCase.Title.Length > 200)
            {
                modCaseSubStringTitle += " [...]";
            }
            embed.WithTitle($"#{modCase.CaseId} {modCaseSubStringTitle}");

            embed.WithDescription(modCase.Description.Substring(0, Math.Min(modCase.Description.Length, 2000)));

            embed.AddField($"{SCALES_EMOTE} - Punishment", modCase.GetPunishment(_translator), true);

            if (modCase.PunishedUntil != null)
            {
                embed.AddField($"{ALARM_CLOCK} - Punished Until (UTC)", modCase.PunishedUntil.Value.ToString("dd.MM.yyyy HH:mm:ss"), true);
            }

            if (modCase.Labels.Length > 0)
            {
                StringBuilder labels = new StringBuilder();
                foreach (string label in modCase.Labels)
                {
                    if (labels.ToString().Length + label.Length + 2 > 2000)
                    {
                        break;
                    }
                    labels.Append($"`{label}` ");
                }
                embed.AddField($"{SCROLL_EMOTE} - Labels", labels.ToString(), false);
            }

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed.Build()));
        }
    }
}