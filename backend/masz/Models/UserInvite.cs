using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using masz.Dtos.DiscordAPIResponses;

namespace masz.Models
{
    public class UserInvite
    {
        [Key]
        public int Id { get; set; }
        public string GuildId { get; set; }
        public string TargetChannelId { get; set; }
        public string JoinedUserId { get; set; }
        public string UsedInvite { get; set; }
        public string InviteIssuerId { get; set; }
        public DateTime JoinedAt { get; set; }
        public DateTime InviteCreatedAt { get; set; }
    }
}
