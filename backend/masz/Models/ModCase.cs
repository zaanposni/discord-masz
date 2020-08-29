using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using masz.Dtos.ModCase;

namespace masz.Models
{
    public class ModCase
    {
        [Key]
        public int Id { get; set; }
        public string GuildId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string UserId { get; set; }
        public string CurrentUsername { get; set; }
        public string CurrentNickname { get; set; }
        public string ModId { get; set; }
        public int Severity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime OccuredAt { get; set; }
        public DateTime LastEditedAt { get; set; }
        public string LastEditedByModId { get; set; }
        public string Punishment { get; set; }
        public string Labels { get; set; }
        public string Others { get; set; }
        public bool Valid { get; set; }
        public virtual List<ModCaseComments> ModCaseComments { get; set; }
    }
}