using System;

namespace masz.Models
{
    public class ModCaseComments
    {
        public int Id { get; set; }
        public int ModCaseId { get; set; }
        public string Content { get; set; }
        public int ModId { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual ModCase ModCase { get; set; }
    }
}