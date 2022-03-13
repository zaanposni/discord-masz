using MASZ.Enums;

namespace MASZ.Models.Views
{
    public class GuildConfigView
    {
        public int Id { get; set; }
        public string GuildId { get; set; }
        public string[] ModRoles { get; set; }
        public string[] AdminRoles { get; set; }
        public string[] MutedRoles { get; set; }
        public bool ModNotificationDM { get; set; }
        public string ModPublicNotificationWebhook { get; set; }
        public string ModInternalNotificationWebhook { get; set; }
        public bool StrictModPermissionCheck { get; set; }
        public bool ExecuteWhoisOnJoin { get; set; }
        public bool PublishModeratorInfo { get; set; }
        public Language PreferredLanguage { get; set; }
        public int AllowBanAppealAfterDays { get; set; }
        public bool PublicEmbedMode { get; set; }


        public GuildConfigView() { }
        public GuildConfigView(GuildConfig config)
        {
            Id = config.Id;
            GuildId = config.GuildId.ToString();
            ModRoles = config.ModRoles.Select(x => x.ToString()).ToArray();
            AdminRoles = config.AdminRoles.Select(x => x.ToString()).ToArray();
            MutedRoles = config.MutedRoles.Select(x => x.ToString()).ToArray();
            ModNotificationDM = config.ModNotificationDM;
            ModPublicNotificationWebhook = config.ModPublicNotificationWebhook;
            ModInternalNotificationWebhook = config.ModInternalNotificationWebhook;
            StrictModPermissionCheck = config.StrictModPermissionCheck;
            ExecuteWhoisOnJoin = config.ExecuteWhoisOnJoin;
            PublishModeratorInfo = config.PublishModeratorInfo;
            PreferredLanguage = config.PreferredLanguage;
            AllowBanAppealAfterDays = config.AllowBanAppealAfterDays;
            PublicEmbedMode = config.PublicEmbedMode;
        }
    }
}
