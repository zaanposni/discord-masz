using DSharpPlus.Entities;

namespace masz.Models
{
    public class CommentListView
    {
        public CommentListView(ModCaseComment comment, DiscordUser commentor)
        {
            Comment = comment;
            Commentor = commentor;
        }
        public ModCaseComment Comment { get; set; }
        public DiscordUser Commentor { get; set; }
    }
}
