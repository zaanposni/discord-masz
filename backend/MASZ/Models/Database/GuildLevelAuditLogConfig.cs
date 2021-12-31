using MASZ.Dtos.GuildLevelAuditLogConfig;
using MASZ.Enums;
using System.ComponentModel.DataAnnotations;

namespace MASZ.Models
{
    public class GuildLevelAuditLogConfig
    {
        [Key]
        public int Id { get; set; }
        public ulong GuildId { get; set; }
        public GuildAuditLogEvent GuildAuditLogEvent { get; set; }
        public ulong ChannelId { get; set; }
        public ulong[] PingRoles { get; set; }

        public GuildLevelAuditLogConfig()
        {
        }

        public GuildLevelAuditLogConfig(GuildLevelAuditLogConfigForPutDto dto, ulong guildId)
        {
            GuildId = guildId;
            GuildAuditLogEvent = dto.GuildAuditLogEvent;
            ChannelId = dto.ChannelId;
            PingRoles = dto.PingRoles;
        }
    }
}
