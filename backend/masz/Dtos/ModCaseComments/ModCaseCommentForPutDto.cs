using System;
using System.ComponentModel.DataAnnotations;

namespace masz.Dtos.ModCaseComments
{
    public class ModCaseCommentForPutDto
    {
        [Required]
        [MaxLength(300)]
        public string Message { get; set; }
    }
}