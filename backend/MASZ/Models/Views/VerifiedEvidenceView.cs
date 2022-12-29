using MASZ.Models.Database;

namespace MASZ.Models.Views
{
    public class VerifiedEvidenceView
    {
        public int Id { get; set; }
        public string GuildId { get; set; }
        public string ChannelId { get; set; }
        public string MessageId { get; set; }
        public string ReportedContent { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public string? Nickname { get; set; }
        public string Discriminator { get; set; }
        public string ModId { get; set; }
        public DateTime SentAt { get; set; }
        public DateTime ReportedAt { get; set; }

        public VerifiedEvidenceView(VerifiedEvidence evidence)
        {
            Id = evidence.Id;
            GuildId = evidence.GuildId.ToString();
            ChannelId = evidence.ChannelId.ToString();
            MessageId = evidence.MessageId.ToString();
            ReportedContent = evidence.ReportedContent;
            UserId = evidence.UserId.ToString();
            Username = evidence.Username;
            Nickname = evidence.Nickname;
            Discriminator = evidence.Discriminator;
            ModId = evidence.ModId.ToString();
            SentAt = evidence.SentAt;
            ReportedAt = evidence.ReportedAt;
        }
    }
}
