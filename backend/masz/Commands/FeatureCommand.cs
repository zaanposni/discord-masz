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
            if (featureTest.HasKickPermission(ctx))
            {
                missingBasicPermissions.Append($"\n- {CHECK} {_translator.T().CmdFeaturesKickPermissionGranted()}");
            } else
            {
                missingBasicPermissions.Append($"\n- {X_CHECK} {_translator.T().CmdFeaturesKickPermissionNotGranted()}");
            }

            // ban
            if (featureTest.HasBanPermission(ctx))
            {
                missingBasicPermissions.Append($"\n- {CHECK} {_translator.T().CmdFeaturesBanPermissionGranted()}");
            } else
            {
                missingBasicPermissions.Append($"\n- {X_CHECK} {_translator.T().CmdFeaturesBanPermissionNotGranted()}");
            }

            // mute
            if (featureTest.HasManagedRolePermission(ctx))
            {
                missingBasicPermissions.Append($"\n- {CHECK} {_translator.T().CmdFeaturesManageRolePermissionGranted()}");
            } else
            {
                missingBasicPermissions.Append($"\n- {X_CHECK} {_translator.T().CmdFeaturesManageRolePermissionNotGranted()}");
            }

            // muted role
            if (! featureTest.HasMutedRolesDefined())
            {
                missingBasicPermissions.Append($"\n- {X_CHECK} {_translator.T().CmdFeaturesMutedRoleUndefined()}");
            } else
            {
                switch (featureTest.HasManagableMutedRoles(ctx)) {
                    case GuildFeatureTestResult.OK:
                        missingBasicPermissions.Append($"\n- {CHECK} {_translator.T().CmdFeaturesMutedRoleDefined()}");
                        break;
                    case GuildFeatureTestResult.ROLE_TOO_HIGH:
                        missingBasicPermissions.Append($"\n- {X_CHECK} {_translator.T().CmdFeaturesMutedRoleDefinedButTooHigh()}");
                        break;
                    default:
                        missingBasicPermissions.Append($"\n- {X_CHECK} {_translator.T().CmdFeaturesMutedRoleDefinedButInvalid()}");
                        break;
                }
            }


            // basic punishment feature
            if (featureTest.FeaturePunishmentExecution(ctx))
            {
                embed.AddField($"{CHECK} {_translator.T().CmdFeaturesPunishmentExecution()}", _translator.T().CmdFeaturesPunishmentExecutionDescription(), false);
            } else
            {
                embed.AddField($"{X_CHECK} {_translator.T().CmdFeaturesPunishmentExecution()}", _translator.T().CmdFeaturesPunishmentExecutionDescription() + missingBasicPermissions.ToString(), false);
            }

            // unban feature
            if (featureTest.HasBanPermission(ctx))
            {
                embed.AddField($"{CHECK} {_translator.T().CmdFeaturesUnbanRequests()}", _translator.T().CmdFeaturesUnbanRequestsDescriptionGranted(), false);
            } else
            {
                embed.AddField($"{X_CHECK} {_translator.T().CmdFeaturesUnbanRequests()}", _translator.T().CmdFeaturesUnbanRequestsDescriptionNotGranted(), false);
            }

            // report command
            if (featureTest.HasInternalWebhookDefined())
            {
                embed.AddField($"{CHECK} {_translator.T().CmdFeaturesReportCommand()}", _translator.T().CmdFeaturesReportCommandDescriptionGranted(), false);
            } else
            {
                embed.AddField($"{X_CHECK} {_translator.T().CmdFeaturesReportCommand()}", _translator.T().CmdFeaturesReportCommandDescriptionNotGranted(), false);
            }

            // invite tracking
            if (featureTest.HasManagedGuildPermission(ctx))
            {
                embed.AddField($"{CHECK} {_translator.T().CmdFeaturesInviteTracking()}", _translator.T().CmdFeaturesInviteTrackingDescriptionGranted(), false);
            } else
            {
                embed.AddField($"{X_CHECK} {_translator.T().CmdFeaturesInviteTracking()}", _translator.T().CmdFeaturesInviteTrackingDescriptionNotGranted(), false);
            }

            if (featureTest.SupportsAllFeatures(ctx))
            {
                embed.WithDescription($"{CHECK} {_translator.T().CmdFeaturesSupportAllFeatures()}");
                embed.WithColor(DiscordColor.Green);
            } else
            {
                embed.WithDescription($"{X_CHECK} {_translator.T().CmdFeaturesMissingFeatures()}");
                embed.WithColor(DiscordColor.Red);
            }

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(embed.Build()));
        }
    }
}