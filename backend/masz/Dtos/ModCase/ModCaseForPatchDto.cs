using System;
using System.ComponentModel.DataAnnotations;

namespace masz.Dtos.ModCase
{
    public class ModCaseForPatchDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string UserId { get; set; }
        public string CurrentUsername { get; set; }
        public int Severity { get; set; }
        public DateTime OccuredAt { get; set; }
        public string Punishment { get; set; }
        public string Labels { get; set; }
        public string Others { get; set; }
        public bool Valid { get; set; }
    }
}