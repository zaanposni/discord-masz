using System;
using System.ComponentModel.DataAnnotations;

namespace masz.Dtos.ModCaseComments
{
    public class ModCaseCommentForPutDto
    {
        [Required(AllowEmptyStrings=false, ErrorMessage="A message is required.")]
        [MaxLength(300)]
        public string Message { get; set; }
    }
}