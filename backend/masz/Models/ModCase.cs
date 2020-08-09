using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace masz.Models
{
    public class ModCase
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }        
        public int GuildId { get; set; }
        public int UserId { get; set; }
        public string CurrentUsername { get; set; }        
        public int ModId { get; set; }
        public int Severity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime OccuredAt { get; set; }
        public DateTime LastEditedAt { get; set; }
        public string Punishment { get; set; }
        public string Labels { get; set; }
        public string Others { get; set; }
        public bool Valid { get; set; }
    }
}