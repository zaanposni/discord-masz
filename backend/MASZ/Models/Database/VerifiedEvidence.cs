using System.ComponentModel.DataAnnotations;

namespace MASZ.Models.Database
{
    public class VerifiedEvidence
    {
        [Key]
        public int Id { get; set; }
        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }
        public ulong MessageId { get; set; }
        public string ReportedContent { get; set; }
        public ulong UserId { get; set; }
        public string Username { get; set; }
        public string? Nickname { get; set; }
        public string Discriminator { get; set; }
        public ulong ModId { get; set; }
        public DateTime SentAt { get; set; }
        public DateTime ReportedAt { get; set; }
        public ICollection<ModCaseEvidenceMapping> EvidenceMappings { get; set; }
    }
}
