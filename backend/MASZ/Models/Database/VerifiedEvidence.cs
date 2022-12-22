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
        public ulong ReporterUserId { get; set; }
        public string ReporterUsername { get; set; }
        public string? ReporterNickname { get; set; }
        public int ReporterDiscriminator { get; set; }
        public ulong ReportedUserId { get; set; }
        public string ReportedUsername { get; set; }
        public string? ReportedNickname { get; set; }
        public int ReportedDiscriminator { get; set; }
        public DateTime SentAt { get; set; }
        public DateTime ReportedAt { get; set; }
        public ICollection<ModCaseEvidenceMapping> EvidenceMappings { get; set; }
    }
}
