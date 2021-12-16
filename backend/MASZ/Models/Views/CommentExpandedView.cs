using Discord;
using MASZ.Models.Views;

namespace MASZ.Models
{
    public class CommentExpandedView
    {
        public CommentExpandedView(ModCaseComment comment, IUser commentor)
        {
            Comment = new CommentsView(comment);
            Commentor = DiscordUserView.CreateOrDefault(commentor);
        }
        public CommentsView Comment { get; set; }
        public DiscordUserView Commentor { get; set; }

        public void RemoveModeratorInfo(string suspectId)
        {
            if (Comment.UserId != suspectId)
            {
                Comment.UserId = null;
                Commentor = null;
            }
        }
    }
}
