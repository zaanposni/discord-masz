using System.ComponentModel.DataAnnotations;

namespace MASZ.Models
{
    public class UserInvite
    {
        [Key]
        public int Id { get; set; }
        public ulong GuildId { get; set; }
        public ulong TargetChannelId { get; set; }
        public ulong JoinedUserId { get; set; }
        public string UsedInvite { get; set; }
        public ulong InviteIssuerId { get; set; }
        public DateTime JoinedAt { get; set; }
        public DateTime? InviteCreatedAt { get; set; }
    }
}
