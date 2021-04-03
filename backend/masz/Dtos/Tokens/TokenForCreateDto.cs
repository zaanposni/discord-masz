using System.ComponentModel.DataAnnotations;

namespace masz.Dtos.Tokens
{
    public class TokenForCreateDto
    {
        [Required(ErrorMessage = "Name field is required", AllowEmptyStrings=false)]
        public string Name { get; set; }
    }
}