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
        public bool HasKickPermission() => _user.GuildPermissions.KickMembers;
        public bool HasBanPermission() => _user.GuildPermissions.BanMembers;
        public bool HasManagedGuildPermission() => _user.GuildPermissions.ManageGuild;
        public bool FeaturePunishmentExecution() => HasKickPermission() &&
                                                                          HasBanPermission() &&
                                                                          HasManagedGuildPermission();
        public bool SupportsAllFeatures() => HasKickPermission() &&
                                                                   HasBanPermission() &&
                                                                   HasManagedGuildPermission() &&
                                                                   HasInternalWebhookDefined();
    }
}
