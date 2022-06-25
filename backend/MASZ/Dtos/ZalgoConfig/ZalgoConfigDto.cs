using System.ComponentModel.DataAnnotations;

namespace MASZ.Dtos
{
    public class ZalgoConfigDto
    {
        public bool Enabled { get; set; }
        [Required]
        [Range(0, 100)]
        public int Percentage { get; set; }
        public bool renameNormal { get; set; }
        [Required]
        [MaxLength(32)]
        public string renameFallback { get; set; } = "zalgo user";
        public bool logToModChannel { get; set; }
    }
}