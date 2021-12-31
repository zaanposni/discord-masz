using System.ComponentModel.DataAnnotations;

namespace MASZ.Dtos.GuildMotd
{
    public class MotdForCreateDto
    {
        [Required(ErrorMessage = "Message field is required", AllowEmptyStrings = false)]
        public string Message { get; set; }
        public bool ShowMotd { get; set; } = true;
    }
}