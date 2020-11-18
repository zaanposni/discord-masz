using System.ComponentModel.DataAnnotations;

namespace masz.Dtos.GuildConfig
{
    public class GuildConfigForPutDto
    {
        [Required(ErrorMessage = "ModRoleId field is required")]
        [RegularExpression(@"^[0-9]{18}$", ErrorMessage = "the role id can only consist of numbers and must be 18 characters long")]
        public string ModRoleId { get; set; }
        [Required(ErrorMessage = "AdminRoleId field is required")]
        [RegularExpression(@"^[0-9]{18}$", ErrorMessage = "the role id can only consist of numbers and must be 18 characters long")]
        public string AdminRoleId { get; set; }
        public bool ModNotificationDM { get; set; }
        [Url(ErrorMessage = "Webhook needs to be a valid url")]
        public string ModPublicNotificationWebhook { get; set; }
        [Url(ErrorMessage = "Webhook needs to be a valid url")]
        public string ModInternalNotificationWebhook { get; set; }
    }
}