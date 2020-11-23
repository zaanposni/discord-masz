using System;
using System.ComponentModel.DataAnnotations;

namespace masz.Models
{
    public class ManagedPunishment
    {
        [Key]
        public int Id { get; set; }
        public ModCase ModCase { get; set; }
        public int CaseDbId { get; set; }
        public string UserId { get; set; }
        public string GuildId { get; set; }
        public PunishmentType PunishmentType { get; set; }
        public DateTime UntilDate { get; set; }
        public bool Active { get; set; }
    }
}