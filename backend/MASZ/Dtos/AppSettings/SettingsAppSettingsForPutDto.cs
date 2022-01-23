using MASZ.Enums;
using System.ComponentModel.DataAnnotations;

namespace MASZ.Dtos.AppSettings
{
    public class SettingsAppSettingsForPutDto
    {
        [Required(ErrorMessage = "DefaultLanguage field is required")]
        public Language DefaultLanguage { get; set; }
        [RegularExpression("https://discord(app)?.com/api/webhooks/[0-9]+/.+", ErrorMessage = "Must be a valid url")]
        public string? AuditLogWebhookURL { get; set; }
        [Required(ErrorMessage = "PublicFileMode field is required")]
        public bool PublicFileMode { get; set; }
    }
}