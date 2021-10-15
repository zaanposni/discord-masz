using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace masz.Dtos.ModCase
{
    public class UploadedFile
    {
        [Required]
        public IFormFile File { set; get; }
    }
}