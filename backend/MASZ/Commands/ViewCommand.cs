using Discord;
using Discord.Interactions;
using MASZ.Enums;
using MASZ.Exceptions;
using MASZ.Extensions;
using MASZ.Models;
using MASZ.Repositories;
using System.Text;

namespace MASZ.Commands
{

    public class ViewCommand : BaseCommand<ViewCommand>
    {
        private readonly string SCALES_EMOTE = "\u2696";
        private readonly string SCROLL_EMOTE = "\uD83C\uDFF7";
        private readonly string ALARM_CLOCK = "\u23F0";

        [SlashCommand("view", "View details of a modcase.")]
        public async Task View([Summary("id", "the id of the case")] long caseId, [Summary("guildid", "the id of the guild")] string guildId = "")
        {
            ulong parsedGuildId = 0;
            if (Context.Guild == null)
            {
                if (!ulong.TryParse(guildId, out parsedGuildId))
                {
                    await Context.Interaction.RespondAsync(Translator.T().CmdViewInvalidGuildId());
                    return;
                }
            }
            else if (string.IsNullOrEmpty(guildId))
            {
                parsedGuildId = Context.Guild.Id;
            }
            else
            {
                try
                {
                    parsedGuildId = ulong.Parse(guildId);
                }
                catch (Exception)
                {
                    await Context.Interaction.RespondAsync(Translator.T().CmdViewInvalidGuildId());
                    return;
                }
            }
            await Context.Interaction.RespondAsync("Getting modcases...");

            ModCase modCase;
            try
            {
                modCase = await ModCaseRepository.CreateDefault(ServiceProvider, CurrentIdentity).GetModCase(parsedGuildId, (int)caseId);
            }
            catch (ResourceNotFoundException)
            {
                await Context.Interaction.ModifyOriginalResponseAsync(message => message.Content = Translator.T().NotFound());
                return;
            }

            if (!await CurrentIdentity.IsAllowedTo(APIActionPermission.View, modCase))
            {
                await Context.Interaction.ModifyOriginalResponseAsync(message => message.Content = Translator.T().CmdViewNotAllowedToView());
                return;
            }

            EmbedBuilder embed = new();
            embed.WithUrl($"{Config.GetBaseUrl()}/guilds/{modCase.GuildId}/cases/{modCase.CaseId}");
            embed.WithTimestamp(modCase.CreatedAt);
            embed.WithColor(Color.Blue);

            IUser suspect = await DiscordAPI.FetchUserInfo(modCase.UserId, CacheBehavior.Default);
            if (suspect != null)
            {
                embed.WithThumbnailUrl(suspect.GetAvatarOrDefaultUrl());
            }

            embed.WithTitle($"#{modCase.CaseId} {modCase.Title.Truncate(200)}");

            embed.WithDescription(modCase.Description.Truncate(2000));

            embed.AddField($"{SCALES_EMOTE} - {Translator.T().Punishment()}", Translator.T().Enum(modCase.PunishmentType), true);

            if (modCase.PunishedUntil != null)
            {
                embed.AddField($"{ALARM_CLOCK} - {Translator.T().PunishmentUntil()}", modCase.PunishedUntil.Value.ToDiscordTS(), true);
            }

            if (modCase.Labels.Length > 0)
            {
                StringBuilder labels = new();
                foreach (string label in modCase.Labels)
                {
                    if (labels.ToString().Length + label.Length + 2 > 2000)
                    {
                        break;
                    }
                    labels.Append($"`{label}` ");
                }
                embed.AddField($"{SCROLL_EMOTE} - {Translator.T().Labels()}", labels.ToString(), false);
            }

            await Context.Interaction.ModifyOriginalResponseAsync(message => { message.Content = ""; message.Embed = embed.Build(); });
        }
    }
}