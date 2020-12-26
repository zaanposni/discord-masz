using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using masz.Dtos.ModCase;

namespace masz.Models
{
    public class AutoModerationEvent
    {
        [Key]
        public int Id { get; set; }
        public string GuildId { get; set; }
        public AutoModerationType AutoModerationType { get; set; }
        public AutoModerationAction AutoModerationAction { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Nickname { get; set; }
        public string Discriminator { get; set; }
        public string MessageId { get; set; }
        public string MessageContent { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? AssociatedCaseId { get; set; }
    }
}
