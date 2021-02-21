using System;
using System.ComponentModel.DataAnnotations;

namespace masz.Dtos.ModCaseComments
{
    public class ModCaseCommentForCreateDto
    {
        [Required(AllowEmptyStrings=false, ErrorMessage="A message is required.")]
        [MaxLength(300)]
        public string Message { get; set; }
    }
}