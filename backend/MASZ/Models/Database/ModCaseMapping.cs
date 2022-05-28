using System.ComponentModel.DataAnnotations;

namespace MASZ.Models
{
    public class ModCaseMapping
    {
        [Key]
        public int Id { get; set; }
        public ModCase CaseA { get; set; }
        public ModCase CaseB { get; set; }
    }
}