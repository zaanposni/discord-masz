using Discord;
using Discord.Interactions;
using MASZ.Attributes;
using MASZ.Enums;
using MASZ.Exceptions;
using MASZ.Extensions;
using MASZ.Models;
using MASZ.Repositories;
using System.Text;

namespace MASZ.Commands
{

    public class WhoisCommand : BaseCommand<WhoisCommand>
    {
        [Require(RequireCheckEnum.GuildModerator)]
        [SlashCommand("whois", "Whois information about a user.")]
        public async Task Whois([Summary("user", "user to scan")] IUser user)
        {
            await Context.Interaction.RespondAsync("Getting WHOIS information...");

            IGuildUser member = null;
            try
            {
                member = Context.Guild.GetUser(user.Id);
            }
            catch (Exception) { }

            EmbedBuilder embed = new EmbedBuilder()
                .WithFooter($"UserId: {user.Id}")
                .WithTimestamp(DateTime.UtcNow)
                .WithColor(Color.Blue)
                .WithDescription(user.Mention);

            List<UserInvite> invites = await InviteRepository.CreateDefault(ServiceProvider).GetusedInvitesForUserAndGuild(user.Id, Context.Guild.Id);
            List<UserInvite> filteredInvites = invites.OrderByDescending(x => x.JoinedAt).ToList();
            if (member != null && member.JoinedAt != null)
            {
                filteredInvites = filteredInvites.FindAll(x => x.JoinedAt >= member.JoinedAt.Value.UtcDateTime);
            }
            StringBuilder joinedInfo = new();
            if (member != null)
            {
                joinedInfo.AppendLine(member.JoinedAt.Value.DateTime.ToDiscordTS());
            }
            if (filteredInvites.Count > 0)
            {
                UserInvite usedInvite = filteredInvites.First();
                joinedInfo.AppendLine(Translator.T().CmdWhoisUsedInvite(usedInvite.UsedInvite));
                if (usedInvite.InviteIssuerId != 0)
                {
                    joinedInfo.AppendLine(Translator.T().CmdWhoisInviteBy(usedInvite.InviteIssuerId));
                }
            }
            if (!string.IsNullOrEmpty(joinedInfo.ToString()))
            {
                embed.AddField(Translator.T().Joined(), joinedInfo.ToString(), true);
            }
            embed.AddField(Translator.T().Registered(), user.CreatedAt.DateTime.ToDiscordTS(), true);

            embed.WithAuthor(user);
            embed.WithThumbnailUrl(user.GetAvatarOrDefaultUrl(size: 1024));

            try
            {
                UserNote userNote = await UserNoteRepository.CreateDefault(ServiceProvider, CurrentIdentity).GetUserNote(Context.Guild.Id, user.Id);
                embed.AddField(Translator.T().UserNote(), userNote.Description.Truncate(1000), false);
            }
            catch (ResourceNotFoundException) { }

            List<UserMapping> userMappings = await UserMapRepository.CreateDefault(ServiceProvider, CurrentIdentity).GetUserMapsByGuildAndUser(Context.Guild.Id, user.Id);
            if (userMappings.Count > 0)
            {
                StringBuilder userMappingsInfo = new();
                foreach (UserMapping userMapping in userMappings.Take(5))
                {
                    ulong otherUser = userMapping.UserA == user.Id ? userMapping.UserB : userMapping.UserA;
                    userMappingsInfo.AppendLine($"<@{otherUser}> - {userMapping.Reason.Truncate(80)}");
                }
                if (userMappings.Count > 5)
                {
                    userMappingsInfo.Append("[...]");
                }
                embed.AddField($"{Translator.T().UserMaps()} [{userMappings.Count}]", userMappingsInfo.ToString(), false);
            }

            List<ModCase> cases = await ModCaseRepository.CreateWithBotIdentity(ServiceProvider).GetCasesForGuildAndUser(Context.Guild.Id, user.Id);
            List<ModCase> activeCases = cases.FindAll(c => c.PunishmentActive);

            if (cases.Count > 0)
            {
                StringBuilder caseInfo = new();
                foreach (ModCase modCase in cases.Take(5))
                {
                    caseInfo.Append($"[{modCase.CaseId} - {modCase.Title.Truncate(50)}]");
                    caseInfo.Append($"({Config.GetBaseUrl()}/guilds/{modCase.GuildId}/cases/{modCase.CaseId})\n");
                }
                if (cases.Count > 5)
                {
                    caseInfo.Append("[...]");
                }
                embed.AddField($"{Translator.T().Cases()} [{cases.Count}]", caseInfo.ToString(), false);

                if (activeCases.Count > 0)
                {
                    StringBuilder activeInfo = new();
                    foreach (ModCase modCase in activeCases.Take(5))
                    {
                        activeInfo.Append($"{modCase.GetPunishment(Translator)} ");
                        if (modCase.PunishedUntil != null)
                        {
                            activeInfo.Append($"({Translator.T().Until()} {modCase.PunishedUntil.Value.ToDiscordTS()}) ");
                        }
                        activeInfo.Append($"[{modCase.CaseId} - {modCase.Title.Truncate(50)}]");
                        activeInfo.Append($"({Config.GetBaseUrl()}/guilds/{modCase.GuildId}/cases/{modCase.CaseId})\n");
                    }
                    if (activeCases.Count > 5)
                    {
                        activeInfo.Append("[...]");
                    }
                    embed.AddField($"{Translator.T().ActivePunishments()} [{activeCases.Count}]", activeInfo.ToString(), false);
                }
            }
            else
            {
                embed.AddField($"{Translator.T().Cases()} [0]", Translator.T().CmdWhoisNoCases(), false);
            }

            await Context.Interaction.ModifyOriginalResponseAsync(message => { message.Content = ""; message.Embed = embed.Build(); });
        }
    }
}