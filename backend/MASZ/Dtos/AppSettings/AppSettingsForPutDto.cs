using MASZ.Enums;
using System.ComponentModel.DataAnnotations;

namespace MASZ.Dtos.AppSettings
{
    public class AppSettingsForPutDto
    {
        [Required(ErrorMessage = "EmbedTitle field is required")]
        [MaxLength(256)]
        public string EmbedTitle { get; set; }
        [Required(ErrorMessage = "EmbedContent field is required")]
        [MaxLength(4096)]
        public string EmbedContent { get; set; }
    }
}