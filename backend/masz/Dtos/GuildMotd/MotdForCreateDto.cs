using System.ComponentModel.DataAnnotations;

namespace masz.Dtos.GuildConfig
{
    public class MotdForCreateDto
    {
        [Required(ErrorMessage = "Message field is required", AllowEmptyStrings=false)]
        public string Message { get; set; }
    }
}