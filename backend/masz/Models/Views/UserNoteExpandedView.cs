using DSharpPlus.Entities;
using masz.Models.Views;

namespace masz.Models
{
    public class UserNoteExpandedView
    {
        public UserNoteView UserNote { get; set; }
        public DiscordUserView User { get; set; }
        public DiscordUserView Moderator { get; set; }
        public UserNoteExpandedView(UserNote userNote, DiscordUser user, DiscordUser moderator)
        {
            UserNote = new UserNoteView(userNote);
            User = DiscordUserView.CreateOrDefault(user);
            Moderator = DiscordUserView.CreateOrDefault(moderator);
        }
    }
}
