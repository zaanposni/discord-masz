using System.ComponentModel.DataAnnotations;

namespace masz.Dtos.GuildConfig
{
    public class GuildConfigForCreateDto
    {
        [Required(ErrorMessage = "ModRoleId field is required")]
        public string ModRoleId { get; set; }
        [Required(ErrorMessage = "AdminRoleId field is required")]
        public string AdminRoleId { get; set; }
        public bool ModNotificationDM { get; set; }
        [Url(ErrorMessage = "Webhook needs to be a valid url")]
        public string ModPublicNotificationWebhook { get; set; }
        [Url(ErrorMessage = "Webhook needs to be a valid url")]
        public string ModInternalNotificationWebhook { get; set; }
    }
}