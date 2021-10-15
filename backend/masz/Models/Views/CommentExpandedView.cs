using DSharpPlus.Entities;
using masz.Models.Views;

namespace masz.Models
{
    public class CommentExpandedView
    {
        public CommentExpandedView(ModCaseComment comment, DiscordUser commentor)
        {
            Comment = new CommentsView(comment);
            Commentor = DiscordUserView.CreateOrDefault(commentor);
        }
        public CommentsView Comment { get; set; }
        public DiscordUserView Commentor { get; set; }

        public void RemoveModeratorInfo(string suspectId)
        {
            if (Comment.UserId != suspectId) {
                Comment.UserId = null;
                Commentor = null;
            }
        }
    }
}
