using Discord;
using Discord.Interactions;
using MASZ.Attributes;
using MASZ.Enums;
using MASZ.Models;
using MASZ.Repositories;
using System.Text;

namespace MASZ.Commands
{

    public class FeatureCommand : BaseCommand<FeatureCommand>
    {
        private readonly string CHECK = "\u2705";
        private readonly string X_CHECK = "\u274C";

        [Require(RequireCheckEnum.GuildModerator)]
        [SlashCommand("features", "Checks if further configuration is needed to use MASZ features.")]
        public async Task Features()
        {
            GuildConfig guildConfig = await GuildConfigRepository.CreateDefault(ServiceProvider).GetGuildConfig(Context.Guild.Id);

            GuildFeatureTest featureTest = new(guildConfig, Context.Guild.CurrentUser);

            EmbedBuilder embed = new();
            embed.WithTimestamp(DateTime.UtcNow);

            StringBuilder missingBasicPermissions = new();

            // kick
            if (featureTest.HasKickPermission())
            {
                missingBasicPermissions.Append($"\n- {CHECK} {Translator.T().CmdFeaturesKickPermissionGranted()}");
            }
            else
            {
                missingBasicPermissions.Append($"\n- {X_CHECK} {Translator.T().CmdFeaturesKickPermissionNotGranted()}");
            }

            // ban
            if (featureTest.HasBanPermission())
            {
                missingBasicPermissions.Append($"\n- {CHECK} {Translator.T().CmdFeaturesBanPermissionGranted()}");
            }
            else
            {
                missingBasicPermissions.Append($"\n- {X_CHECK} {Translator.T().CmdFeaturesBanPermissionNotGranted()}");
            }

            // basic punishment feature
            if (featureTest.FeaturePunishmentExecution())
            {
                embed.AddField($"{CHECK} {Translator.T().CmdFeaturesPunishmentExecution()}", Translator.T().CmdFeaturesPunishmentExecutionDescription(), false);
            }
            else
            {
                embed.AddField($"{X_CHECK} {Translator.T().CmdFeaturesPunishmentExecution()}", Translator.T().CmdFeaturesPunishmentExecutionDescription() + missingBasicPermissions.ToString(), false);
            }

            // unban feature
            if (featureTest.HasBanPermission())
            {
                embed.AddField($"{CHECK} {Translator.T().CmdFeaturesUnbanRequests()}", Translator.T().CmdFeaturesUnbanRequestsDescriptionGranted(), false);
            }
            else
            {
                embed.AddField($"{X_CHECK} {Translator.T().CmdFeaturesUnbanRequests()}", Translator.T().CmdFeaturesUnbanRequestsDescriptionNotGranted(), false);
            }

            // report command
            if (featureTest.HasInternalWebhookDefined())
            {
                embed.AddField($"{CHECK} {Translator.T().CmdFeaturesReportCommand()}", Translator.T().CmdFeaturesReportCommandDescriptionGranted(), false);
            }
            else
            {
                embed.AddField($"{X_CHECK} {Translator.T().CmdFeaturesReportCommand()}", Translator.T().CmdFeaturesReportCommandDescriptionNotGranted(), false);
            }

            // invite tracking
            if (featureTest.HasManagedGuildPermission())
            {
                embed.AddField($"{CHECK} {Translator.T().CmdFeaturesInviteTracking()}", Translator.T().CmdFeaturesInviteTrackingDescriptionGranted(), false);
            }
            else
            {
                embed.AddField($"{X_CHECK} {Translator.T().CmdFeaturesInviteTracking()}", Translator.T().CmdFeaturesInviteTrackingDescriptionNotGranted(), false);
            }

            if (featureTest.SupportsAllFeatures())
            {
                embed.WithTitle(Translator.T().CmdFeaturesSupportAllFeatures())
                    .WithDescription(Translator.T().CmdFeaturesSupportAllFeaturesDesc())
                    .WithColor(Color.Green);
            }
            else
            {
                embed.WithTitle(Translator.T().CmdFeaturesMissingFeatures())
                    .WithColor(Color.Red);
            }

            await Context.Interaction.RespondAsync(embed: embed.Build());
        }
    }
}