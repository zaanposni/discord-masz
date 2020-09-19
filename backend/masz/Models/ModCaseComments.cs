using System;

namespace masz.Models
{
    public class ModCaseComments
    {
        public int Id { get; set; }
        public string ModCaseGuildId { get; set; }
        public int ModCaseId { get; set; }
        public string Content { get; set; }
        public string ModId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastEditedAt { get; set; }
        public virtual ModCase ModCase { get; set; }
    }
}