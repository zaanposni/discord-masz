using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using masz.Dtos.ModCase;

namespace masz.Models
{
    public class ModCase : ICloneable
    {
        [Key]
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
        public string Punishment { get; set; }
        public string[] Labels { get; set; }
        public string Others { get; set; }
        public bool Valid { get; set; }
        public CaseCreationType CreationType { get; set; }
        public PunishmentType PunishmentType { get; set; }
        public DateTime? PunishedUntil { get; set; }
        public bool PunishmentActive { get; set; }
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
                Punishment = this.Punishment,
                Labels = this.Labels,
                Others = this.Others,
                Valid = this.Valid,
                PunishmentType = this.PunishmentType,
                PunishedUntil = this.PunishedUntil,
                PunishmentActive = this.PunishmentActive,
                Comments = this.Comments
            };
        }
    }
}