using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using masz.Enums;
using masz.Exceptions;
using masz.Extensions;
using masz.Models;
using masz.Repositories;

namespace masz.Commands
{

    public class WhoisCommand : BaseCommand<WhoisCommand>
    {
        public WhoisCommand(IServiceProvider serviceProvider) : base(serviceProvider) { }

        [SlashCommand("whois", "Whois information about a user.")]
        public async Task Whois(InteractionContext ctx,  [Option("user", "user to scan")] DiscordUser user)
        {
            await Require(ctx, RequireCheckEnum.GuildModerator);
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

            DiscordMember member = await ctx.Guild.GetMemberAsync(user.Id);

            DiscordEmbedBuilder embed = new DiscordEmbedBuilder();
            embed.WithFooter($"UserId: {user.Id}");
            embed.WithTimestamp(DateTime.UtcNow);
            embed.WithDescription(user.Mention);

            List<UserInvite> invites = await InviteRepository.CreateDefault(_serviceProvider).GetusedInvitesForUserAndGuild(user.Id, ctx.Guild.Id);
            List<UserInvite> filteredInvites = invites.OrderByDescending(x => x.JoinedAt).ToList();
            if (member != null && member.JoinedAt != null)
            {
                filteredInvites = filteredInvites.FindAll(x => x.JoinedAt >= member.JoinedAt.UtcDateTime);
            }
            StringBuilder joinedInfo = new StringBuilder();
            if (member != null)
            {
                joinedInfo.AppendLine(member.JoinedAt.DateTime.ToDiscordTS());
            }
            if (filteredInvites.Count > 0)
            {
                UserInvite usedInvite = filteredInvites.First();
                joinedInfo.AppendLine(_translator.T().CmdWhoisUsedInvite(usedInvite.UsedInvite));
                if (usedInvite.InviteIssuerId != 0)
                {
                    joinedInfo.AppendLine(_translator.T().CmdWhoisInviteBy(usedInvite.InviteIssuerId));
                }
            }
            if (! string.IsNullOrEmpty(joinedInfo.ToString()))
            {
                embed.AddField(_translator.T().Joined(), joinedInfo.ToString(), true);
            }
            embed.AddField(_translator.T().Registered(), user.CreationTimestamp.DateTime.ToDiscordTS(), true);

            embed.WithAuthor(user.Username, user.AvatarUrl, user.AvatarUrl);
            embed.WithThumbnail(user.AvatarUrl);

            try
            {
                UserNote userNote = await UserNoteRepository.CreateDefault(_serviceProvider, _currentIdentity).GetUserNote(ctx.Guild.Id, user.Id);
                embed.AddField(_translator.T().UserNote(), userNote.Description.Truncate(1000), false);
            } catch (ResourceNotFoundException) { }

            List<ModCase> cases = await ModCaseRepository.CreateWithBotIdentity(_serviceProvider).GetCasesForGuildAndUser(ctx.Guild.Id, user.Id);
            List<ModCase> activeCases = cases.FindAll(c => c.PunishmentActive);

            if (cases.Count > 0)
            {
                StringBuilder caseInfo = new StringBuilder();
                foreach (ModCase modCase in cases.Take(5))
                {
                    caseInfo.Append($"[{modCase.CaseId} - {modCase.Title}]");
                    caseInfo.Append($"({_config.GetBaseUrl()}/guilds/{modCase.GuildId}/cases/{modCase.CaseId})\n");
                }
                if (cases.Count > 5)
                {
                    caseInfo.Append("[...]");
                }
                embed.AddField($"{_translator.T().Cases()} [{cases.Count}]", caseInfo.ToString(), false);

                if (activeCases.Count > 0)
                {
                    StringBuilder activeInfo = new StringBuilder();
                    foreach (ModCase modCase in activeCases.Take(5))
                    {
                        activeInfo.Append($"{modCase.GetPunishment(_translator)} ");
                        if (modCase.PunishedUntil != null)
                        {
                            activeInfo.Append($"({_translator.T().Until()} {modCase.PunishedUntil.Value.ToDiscordTS()}) ");
                        }
                        activeInfo.Append($"[{modCase.CaseId} - {modCase.Title}]");
                        activeInfo.Append($"({_config.GetBaseUrl()}/guilds/{modCase.GuildId}/cases/{modCase.CaseId})\n");
                    }
                    if (activeCases.Count > 5)
                    {
                        activeInfo.Append("[...]");
                    }
                    embed.AddField($"{_translator.T().ActivePunishments()} [{activeCases.Count}]", activeInfo.ToString(), false);
                }
            } else
            {
                embed.AddField($"{_translator.T().Cases()} [0]", _translator.T().CmdWhoisNoCases(), false);
            }

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed.Build()));
        }
    }
}