using Discord;

namespace MASZ.Models
{
    public class CommentExpandedTableView : CommentExpandedView
    {
        public CommentExpandedTableView(ModCaseComment comment, IUser commentor, ulong guildId, int caseId) : base(comment, commentor)
        {
            GuildId = guildId.ToString();
            CaseId = caseId;
        }
        public string GuildId { get; set; }
        public int CaseId { get; set; }
    }
}
