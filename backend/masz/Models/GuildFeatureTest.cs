
using System;
using System.Linq;
using System.Net.Mime;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using masz.Enums;

namespace masz.Models
{
    public class GuildFeatureTest
    {
        private GuildConfig _guildConfig;

        public GuildFeatureTest(GuildConfig guildConfig)
        {
            _guildConfig = guildConfig;
        }

        public bool HasInternalWebhookDefined() => !string.IsNullOrEmpty(_guildConfig.ModInternalNotificationWebhook);
        public bool HasMutedRolesDefined() => _guildConfig.MutedRoles.Length > 0;
        public GuildFeatureTestResult HasManagableMutedRoles(InteractionContext ctx)
        {
            foreach (var item in _guildConfig.MutedRoles)
            {
                try
                {
                    DiscordRole role = ctx.Guild.GetRole(item);
                    if (role == null)
                    {
                        return GuildFeatureTestResult.ROLE_UNKNOWN;
                    }
                    if (role.Position >= ctx.Guild.CurrentMember.Roles.Max(r => r.Position))
                    {
                        return GuildFeatureTestResult.ROLE_TOO_HIGH;
                    }
                } catch (Exception)
                {
                    return GuildFeatureTestResult.ROLE_UNKNOWN;
                }
            }
            return GuildFeatureTestResult.OK;
        }
        public bool HasKickPermission(InteractionContext ctx) => ((int) ctx.Guild.CurrentMember.Permissions & 1 << (int) DiscordBitPermissionFlag.KICK_MEMBERS) >= 1;
        public bool HasBanPermission(InteractionContext ctx) => ((int) ctx.Guild.CurrentMember.Permissions & 1 << (int) DiscordBitPermissionFlag.BAN_MEMBERS) >= 1;
        public bool HasManagedRolePermission(InteractionContext ctx) => ((int) ctx.Guild.CurrentMember.Permissions & 1 << (int) DiscordBitPermissionFlag.MANAGE_ROLES) >= 1;
        public bool HasManagedGuildPermission(InteractionContext ctx) => ((int) ctx.Guild.CurrentMember.Permissions & 1 << (int) DiscordBitPermissionFlag.MANAGE_GUILD) >= 1;
        public bool FeaturePunishmentExecution(InteractionContext ctx) => HasKickPermission(ctx) &&
                                                                          HasBanPermission(ctx) &&
                                                                          HasManagedRolePermission(ctx) &&
                                                                          HasManagedGuildPermission(ctx) &&
                                                                          HasMutedRolesDefined() &&
                                                                          HasManagableMutedRoles(ctx) == GuildFeatureTestResult.OK;
        public bool SupportsAllFeatures(InteractionContext ctx) => HasKickPermission(ctx) &&
                                                                   HasBanPermission(ctx) &&
                                                                   HasManagedRolePermission(ctx) &&
                                                                   HasManagedGuildPermission(ctx) &&
                                                                   HasMutedRolesDefined() &&
                                                                   HasManagableMutedRoles(ctx) == GuildFeatureTestResult.OK &&
                                                                   HasInternalWebhookDefined();
    }
}
