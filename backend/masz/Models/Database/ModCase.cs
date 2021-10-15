using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using masz.Services;
using masz.Enums;

namespace masz.Models
{
    public class ModCase : ICloneable
    {
        [Key]
        public int Id { get; set; }
        public int CaseId { get; set; }
        public ulong GuildId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public ulong UserId { get; set; }
        public string Username { get; set; }
        public string Discriminator { get; set; }
        public string Nickname { get; set; }
        public ulong ModId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime OccuredAt { get; set; }
        public DateTime LastEditedAt { get; set; }
        public ulong LastEditedByModId { get; set; }
        public string[] Labels { get; set; }
        public string Others { get; set; }
        public bool Valid { get; set; }
        public CaseCreationType CreationType { get; set; }
        public PunishmentType PunishmentType { get; set; }
        public DateTime? PunishedUntil { get; set; }
        public bool PunishmentActive { get; set; }
        public bool AllowComments { get; set; }
        public ulong LockedByUserId { get; set; }
        public DateTime? LockedAt { get; set; }
        public DateTime? MarkedToDeleteAt { get; set; }
        public ulong DeletedByUserId { get; set; }
        public ICollection<ModCaseComment> Comments { get; set; }

        public object Clone()
        {
            return new ModCase {
                Id = this.Id,
                CaseId = this.CaseId,
                GuildId = this.GuildId,
                Title = this.Title,
                Description = this.Description,
                UserId = this.UserId,
                Username = this.Username,
                Nickname = this.Nickname,
                ModId = this.ModId,
                CreatedAt = this.CreatedAt,
                OccuredAt = this.OccuredAt,
                LastEditedAt = this.LastEditedAt,
                LastEditedByModId = this.LastEditedByModId,
                Labels = this.Labels,
                Others = this.Others,
                Valid = this.Valid,
                PunishmentType = this.PunishmentType,
                PunishedUntil = this.PunishedUntil,
                PunishmentActive = this.PunishmentActive,
                AllowComments = this.AllowComments,
                LockedByUserId = this.LockedByUserId,
                LockedAt = this.LockedAt,
                MarkedToDeleteAt = this.MarkedToDeleteAt,
                DeletedByUserId = this.DeletedByUserId,
                Comments = this.Comments,
            };
        }

        public string GetPunishment(ITranslator translator)
        {
            return translator.T().Enum(PunishmentType);
        }
    }
}