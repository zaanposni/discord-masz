using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using masz.Dtos.DiscordAPIResponses;

namespace masz.Models
{
    public class CaseTemplate
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string TemplateName { get; set; }
        public string CreatedForGuildId { get; set; }
        public ViewPermission ViewPermission { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CaseTitle { get; set; }
        public string CaseDescription { get; set; }
        public string[] CaseLabels { get; set; }
        public PunishmentType CasePunishmentType { get; set; }
        public DateTime? CasePunishedUntil { get; set; }
        public bool sendPublicNotification { get; set; }
        public bool handlePunishment { get; set; }
        public bool announceDm { get; set; }
    }
}
