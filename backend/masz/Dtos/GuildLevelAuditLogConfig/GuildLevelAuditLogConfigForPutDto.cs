using MASZ.Enums;
using System.ComponentModel.DataAnnotations;

namespace MASZ.Dtos.GuildLevelAuditLogConfig
{
    public class GuildLevelAuditLogConfigForPutDto
    {
        [Required]
        public GuildAuditLogEvent GuildAuditLogEvent { get; set; }
        public ulong ChannelId { get; set; }
        public ulong[] PingRoles { get; set; }
    }
}