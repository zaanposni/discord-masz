using System.ComponentModel.DataAnnotations;

namespace MASZ.Dtos.Tokens
{
    public class TokenForCreateDto
    {
        [Required(ErrorMessage = "Name field is required", AllowEmptyStrings = false)]
        public string Name { get; set; }
    }
}