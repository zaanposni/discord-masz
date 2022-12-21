using System.ComponentModel.DataAnnotations;

namespace MASZ.Models.Database
{
    public class VerifiedEvidence
    {
        [Key]
        public int Id { get; set; }
        public ulong GuildId { get; set; }
        public ulong MessageId { get; set; }
        public string ReportedContent { get; set; }
        public ICollection<ModCaseEvidenceMapping> EvidenceMappings { get; set; }
    }
}
