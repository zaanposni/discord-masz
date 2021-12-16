using Discord;
using MASZ.Models.Views;

namespace MASZ.Models
{
    public class UserNoteExpandedView
    {
        public UserNoteView UserNote { get; set; }
        public DiscordUserView User { get; set; }
        public DiscordUserView Moderator { get; set; }
        public UserNoteExpandedView(UserNote userNote, IUser user, IUser moderator)
        {
            UserNote = new UserNoteView(userNote);
            User = DiscordUserView.CreateOrDefault(user);
            Moderator = DiscordUserView.CreateOrDefault(moderator);
        }
    }
}
