using Discord;
using MASZ.Enums;

namespace MASZ.Models
{
    public class GuildFeatureTest
    {
        private readonly GuildConfig _guildConfig;
        private readonly IGuildUser _user;

        public GuildFeatureTest(GuildConfig guildConfig, IGuildUser user)
        {
            _guildConfig = guildConfig;
            _user = user;
        }

        public bool HasInternalWebhookDefined() => !string.IsNullOrEmpty(_guildConfig.ModInternalNotificationWebhook);
        public bool HasMutedRolesDefined() => _guildConfig.MutedRoles.Length > 0;
        public GuildFeatureTestResult HasManagableMutedRoles()
        {
            foreach (var item in _guildConfig.MutedRoles)
            {
                try
                {
                    IRole role = _user.Guild.GetRole(item);
                    if (role == null)
                    {
                        return GuildFeatureTestResult.RoleUnknown;
                    }
                    if (role.Position >= _user.RoleIds.Select(r => _user.Guild.Roles.Where(role => role.Id == r).First()).Max(r => r.Position))
                    {
                        return GuildFeatureTestResult.RoleTooHigh;
                    }
                }
                catch (Exception)
                {
                    return GuildFeatureTestResult.RoleUnknown;
                }
            }
            return GuildFeatureTestResult.Ok;
        }
        public bool HasKickPermission() => _user.GuildPermissions.KickMembers;
        public bool HasBanPermission() => _user.GuildPermissions.BanMembers;
        public bool HasManagedRolePermission() => _user.GuildPermissions.ManageRoles;
        public bool HasManagedGuildPermission() => _user.GuildPermissions.ManageGuild;
        public bool FeaturePunishmentExecution() => HasKickPermission() &&
                                                                          HasBanPermission() &&
                                                                          HasManagedRolePermission() &&
                                                                          HasManagedGuildPermission() &&
                                                                          HasMutedRolesDefined() &&
                                                                          HasManagableMutedRoles() == GuildFeatureTestResult.Ok;
        public bool SupportsAllFeatures() => HasKickPermission() &&
                                                                   HasBanPermission() &&
                                                                   HasManagedRolePermission() &&
                                                                   HasManagedGuildPermission() &&
                                                                   HasMutedRolesDefined() &&
                                                                   HasManagableMutedRoles() == GuildFeatureTestResult.Ok &&
                                                                   HasInternalWebhookDefined();
    }
}
