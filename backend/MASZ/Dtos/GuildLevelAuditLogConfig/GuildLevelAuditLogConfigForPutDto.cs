using MASZ.Enums;
using System.ComponentModel.DataAnnotations;

namespace MASZ.Dtos.GuildLevelAuditLogConfig
{
    public class GuildLevelAuditLogConfigForPutDto
    {
        [Required]
        public GuildAuditLogEvent GuildAuditLogEvent { get; set; }
        [Required]
        public ulong ChannelId { get; set; }
        [Required]
        public ulong[] PingRoles { get; set; }
        [Required]
        public ulong[] IgnoreRoles { get; set; }
        [Required]
        public ulong[] IgnoreChannels { get; set; }
    }
}