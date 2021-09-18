using System;

namespace masz.Models.Views
{
    public class CaseTemplateView
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
        public CaseTemplateView() { }
        public CaseTemplateView(CaseTemplate template)
        {
            Id = template.Id;
            UserId = template.UserId.ToString();
            TemplateName = template.TemplateName;
            CreatedForGuildId = template.CreatedForGuildId.ToString();
            ViewPermission = template.ViewPermission;
            CreatedAt = template.CreatedAt;
            CaseTitle = template.CaseTitle;
            CaseDescription = template.CaseDescription;
            CaseLabels = template.CaseLabels;
            CasePunishmentType = template.CasePunishmentType;
            CasePunishedUntil = template.CasePunishedUntil;
            sendPublicNotification = template.sendPublicNotification;
            handlePunishment = template.handlePunishment;
            announceDm = template.announceDm;
        }
    }
}
