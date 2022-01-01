using MASZ.Enums;
using System.ComponentModel.DataAnnotations;

namespace MASZ.Dtos.AppSettings
{
    public class SEttingsAppSettingsForPutDto
    {
        [Required(ErrorMessage = "DefaultLanguage field is required")]
        public Language DefaultLanguage { get; set; }
        [Url(ErrorMessage = "Must be a valid url")]
        public string AuditLogWebhookURL { get; set; }
    }
}