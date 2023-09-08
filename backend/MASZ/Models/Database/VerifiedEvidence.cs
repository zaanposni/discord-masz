using System.ComponentModel.DataAnnotations;
using Discord;
using MASZ.Extensions;

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
        public ulong ModId { get; set; }
        public DateTime SentAt { get; set; }
        public DateTime ReportedAt { get; set; }
        public ICollection<ModCaseEvidenceMapping> EvidenceMappings { get; set; }

        public string GetEmbedData(string baseUrl, IUser user)
        {
            return
                "<html>" +
                    "<head>" +
                        $"<meta name=\"theme-color\" content=\"#3498db\">" +
                        $"<meta property=\"og:site_name\" content=\"MASZ by zaanposni\" />" +
                        $"<meta property=\"og:title\" content=\"#{user.Username} at {this.SentAt.ToString("yyyy.MM.dd")}\" />" +
                        $"<meta property=\"og:url\" content=\"{baseUrl}/guilds/{this.GuildId}/evidence/{this.Id}\" />" +
                        $"<meta property=\"og:description\" content=\"{this.ReportedContent}\" />" +
                        ( user != null ? $"<meta property=\"og:image\" content=\"{user.GetAvatarOrDefaultUrl()}\" />" : "") +
                    "</head>" +
                "</html>";
        }
    }
}
