using MASZ.Enums;

namespace MASZ.Models
{
    public class CaseTemplate
    {
        public int Id { get; set; }
        public ulong UserId { get; set; }
        public string TemplateName { get; set; }
        public ulong CreatedForGuildId { get; set; }
        public ViewPermission ViewPermission { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CaseTitle { get; set; }
        public string CaseDescription { get; set; }
        public string[] CaseLabels { get; set; }
        public PunishmentType CasePunishmentType { get; set; }
        public DateTime? CasePunishedUntil { get; set; }
        public bool SendPublicNotification { get; set; }
        public bool HandlePunishment { get; set; }
        public bool AnnounceDm { get; set; }
    }
}
