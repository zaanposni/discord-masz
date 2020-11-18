using System;
using System.ComponentModel.DataAnnotations;

namespace masz.Dtos.ModCaseComments
{
    public class ModCaseCommentForCreateDto
    {
        [Required]
        [MaxLength(300)]
        public string Message { get; set; }
    }
}