using MASZ.Enums;

namespace MASZ.Models
{
    public class GuildLevelAuditLogConfigView
    {
        public int Id { get; set; }
        public string GuildId { get; set; }
        public GuildAuditLogEvent GuildAuditLogEvent { get; set; }
        public string ChannelId { get; set; }
        public string[] PingRoles { get; set; }
        public string[] IgnoreRoles { get; set; }
        public string[] IgnoreChannels { get; set; }
        public GuildLevelAuditLogConfigView(GuildLevelAuditLogConfig config)
        {
            Id = config.Id;
            GuildId = config.GuildId.ToString();
            GuildAuditLogEvent = config.GuildAuditLogEvent;
            ChannelId = config.ChannelId.ToString();
            PingRoles = config.PingRoles?.Select(x => x.ToString())?.ToArray() ?? new string[0];
            IgnoreRoles = config.IgnoreRoles?.Select(x => x.ToString())?.ToArray() ?? new string[0];
            IgnoreChannels = config.IgnoreChannels?.Select(x => x.ToString())?.ToArray() ?? new string[0];
        }
    }
}
