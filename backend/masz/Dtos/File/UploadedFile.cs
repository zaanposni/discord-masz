using System.ComponentModel.DataAnnotations;

namespace MASZ.Dtos.ModCase
{
    public class UploadedFile
    {
        [Required]
        public IFormFile File { set; get; }
    }
}