using System.ComponentModel.DataAnnotations;

namespace masz.Dtos.GuildConfig
{
    public class GuildConfigForCreateDto
    {
        [Required]
        public string ModRoleId { get; set; }
        [Required]
        public string AdminRoleId { get; set; }
        public string ModNotificationWebhook { get; set; }
    }
}