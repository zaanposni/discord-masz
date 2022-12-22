using MASZ.Models.Database;

namespace MASZ.Models.Views
{
    public class VerifiedEvidenceView
    {
        public int Id { get; set; }
        public string GuildId { get; set; }
        public string MessageId { get; set; }
        public string ReportedContent { get; set; }
        public string ReporterUserId { get; set; }
        public string ReporterUsername { get; set; }
        public string? ReporterNickname { get; set; }
        public int ReporterDiscriminator { get; set; }
        public string ReportedUserId { get; set; }
        public string ReportedUsername { get; set; }
        public string? ReportedNickname { get; set; }
        public int ReportedDiscriminator { get; set; }
        public DateTime SentAt { get; set; }
        public DateTime ReportedAt { get; set; }

        public VerifiedEvidenceView(VerifiedEvidence evidence)
        {
            Id = evidence.Id;
            GuildId = evidence.GuildId.ToString();
            MessageId = evidence.MessageId.ToString();
            ReportedContent = evidence.ReportedContent;
            ReporterUserId = evidence.ReporterUserId.ToString();
            ReporterUsername = evidence.ReporterUsername;
            ReporterNickname = evidence.ReporterNickname;
            ReporterDiscriminator = evidence.ReporterDiscriminator;
            ReportedUserId = evidence.ReportedUserId.ToString();
            ReportedUsername = evidence.ReportedUsername;
            ReportedNickname = evidence.ReportedNickname;
            ReportedDiscriminator = evidence.ReportedDiscriminator;
            SentAt = evidence.SentAt;
            ReportedAt = evidence.ReportedAt;
        }
    }
}
