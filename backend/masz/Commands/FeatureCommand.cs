using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using masz.Enums;
using masz.Models;
using masz.Repositories;
using Microsoft.Extensions.Logging;

namespace masz.Commands
{

    public class FeatureCommand : BaseCommand<FeatureCommand>
    {
        private readonly string CHECK = "\u2705";
        private readonly string X_CHECK = "\u274C";

        private string ChooseEmote(bool evaluate)
        {
            if (evaluate)
            {
                return CHECK;
            } else
            {
                return X_CHECK;
            }
        }
        public FeatureCommand(IServiceProvider serviceProvider) : base(serviceProvider) { }

        [SlashCommand("features", "Checks if further configuration is needed to use MASZ features.")]
        public async Task Features(InteractionContext ctx)
        {
            await Require(ctx, RequireCheckEnum.GuildModerator);

            GuildConfig guildConfig = await GuildConfigRepository.CreateDefault(_serviceProvider).GetGuildConfig(ctx.Guild.Id);

            GuildFeatureTest featureTest = new GuildFeatureTest(guildConfig);

            DiscordEmbedBuilder embed = new DiscordEmbedBuilder();
            embed.WithTimestamp(DateTime.UtcNow);

            StringBuilder missingBasicPermissions = new StringBuilder();

            // kick
            missingBasicPermissions.Append($"\n- {ChooseEmote(featureTest.HasKickPermission(ctx))} Kick permission ");
            if (! featureTest.HasKickPermission(ctx)) missingBasicPermissions.Append($"**not** ");
            missingBasicPermissions.Append("granted.");

            // ban
            missingBasicPermissions.Append($"\n- {ChooseEmote(featureTest.HasBanPermission(ctx))} Ban permission ");
            if (! featureTest.HasBanPermission(ctx)) missingBasicPermissions.Append($"**not** ");
            missingBasicPermissions.Append("granted.");

            // mute
            missingBasicPermissions.Append($"\n- {ChooseEmote(featureTest.HasManagedRolePermission(ctx))} Manage role permission ");
            if (! featureTest.HasManagedRolePermission(ctx)) missingBasicPermissions.Append($"**not** ");
            missingBasicPermissions.Append("granted.");

            // muted role
            if (! featureTest.HasMutedRolesDefined())
            {
                missingBasicPermissions.Append($"\n- {X_CHECK} Muted role **not** defined.");
            } else
            {
                switch (featureTest.HasManagableMutedRoles(ctx)) {
                    case GuildFeatureTestResult.OK:
                        missingBasicPermissions.Append($"\n- {CHECK} Muted role defined.");
                        break;
                    case GuildFeatureTestResult.ROLE_TOO_HIGH:
                        missingBasicPermissions.Append($"\n- {X_CHECK} Muted role defined but **too high in role hierarchy**.");
                        break;
                    default:
                        missingBasicPermissions.Append($"\n- {X_CHECK} Muted role defined but **invalid**.");
                        break;
                }
            }


            // basic punishment feature
            if (featureTest.FeaturePunishmentExecution(ctx))
            {
                embed.AddField($"{CHECK} Punishment execution", "Let MASZ handle punishments (e.g. tempbans, mutes, etc.).", false);
            } else
            {
                embed.AddField($"{X_CHECK} Punishment execution", "Let MASZ handle punishments (e.g. tempbans, mutes, etc.)." + missingBasicPermissions.ToString(), false);
            }

            // unban feature
            if (featureTest.HasBanPermission(ctx))
            {
                embed.AddField($"{CHECK} Unban requests", "Allows banned members to see their cases and comment on it for unban requests.", false);
            } else
            {
                embed.AddField($"{X_CHECK} Unban requests", "Allows banned members to see their cases and comment on it for unban requests.\nGrant this bot the ban permission to use this feature.", false);
            }

            // report command
            if (featureTest.HasInternalWebhookDefined())
            {
                embed.AddField($"{CHECK} Report command", "Allows members to report messages.", false);
            } else
            {
                embed.AddField($"{X_CHECK} Report command", "Allows members to report messages.\nDefine a internal staff webhook to use this feature.", false);
            }

            // invite tracking
            if (featureTest.HasManagedGuildPermission(ctx))
            {
                embed.AddField($"{CHECK} Invite tracking", "Allows MASZ to track the invites new members are using.", false);
            } else
            {
                embed.AddField($"{X_CHECK} Invite tracking", "Allows MASZ to track the invites new members are using.\nGrant this bot the manage guild permission to use this feature.", false);
            }

            if (featureTest.SupportsAllFeatures(ctx))
            {
                embed.WithDescription($"{CHECK} Your bot on this guild is configured correctly. All features of MASZ can be used.");
                embed.WithColor(DiscordColor.Green);
            } else
            {
                embed.WithDescription($"{X_CHECK} There are features of MASZ that you cannot use right now.");
                embed.WithColor(DiscordColor.Red);
            }

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(embed.Build()));
        }
    }
}