using System.ComponentModel.DataAnnotations;

namespace MASZ.Models.Database
{
    public class ModCaseEvidenceMapping
    {
        [Key]
        public int Id { get; set; }
        public VerifiedEvidence Evidence { get; set; }
        public ModCase ModCase { get; set; }
    }
}
