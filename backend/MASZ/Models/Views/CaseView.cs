using MASZ.Enums;
using MASZ.Services;

namespace MASZ.Models
{
    public class CaseView
    {
        public int Id { get; set; }
        public int CaseId { get; set; }
        public string GuildId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Discriminator { get; set; }
        public string Nickname { get; set; }
        public string ModId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime OccuredAt { get; set; }
        public DateTime LastEditedAt { get; set; }
        public string LastEditedByModId { get; set; }
        public string[] Labels { get; set; }
        public string Others { get; set; }
        public bool Valid { get; set; }
        public CaseCreationType CreationType { get; set; }
        public PunishmentType PunishmentType { get; set; }
        public DateTime? PunishedUntil { get; set; }
        public bool PunishmentActive { get; set; }
        public bool AllowComments { get; set; }
        public string LockedByUserId { get; set; }
        public DateTime? LockedAt { get; set; }
        public DateTime? MarkedToDeleteAt { get; set; }
        public string DeletedByUserId { get; set; }
        public CaseView(ModCase modCase)
        {
            Id = modCase.Id;
            CaseId = modCase.CaseId;
            GuildId = modCase.GuildId.ToString();
            Title = modCase.Title;
            Description = modCase.Description;
            UserId = modCase.UserId.ToString();
            Username = modCase.Username;
            Discriminator = modCase.Discriminator;
            Nickname = modCase.Nickname;
            ModId = modCase.ModId.ToString();
            CreatedAt = modCase.CreatedAt;
            OccuredAt = modCase.OccuredAt;
            LastEditedAt = modCase.LastEditedAt;
            LastEditedByModId = modCase.LastEditedByModId.ToString();
            Labels = modCase.Labels;
            Others = modCase.Others;
            Valid = modCase.Valid;
            CreationType = modCase.CreationType;
            PunishmentType = modCase.PunishmentType;
            PunishedUntil = modCase.PunishedUntil;
            PunishmentActive = modCase.PunishmentActive;
            AllowComments = modCase.AllowComments;
            LockedByUserId = modCase.LockedByUserId.ToString();
            LockedAt = modCase.LockedAt;
            MarkedToDeleteAt = modCase.MarkedToDeleteAt;
            DeletedByUserId = modCase.DeletedByUserId.ToString();
        }

        public void RemoveModeratorInfo()
        {
            ModId = null;
            LastEditedByModId = null;
            LockedByUserId = null;
            DeletedByUserId = null;
        }

        public string GetPunishment(Translator translator)
        {
            return translator.T().Enum(PunishmentType);
        }
    }
}
