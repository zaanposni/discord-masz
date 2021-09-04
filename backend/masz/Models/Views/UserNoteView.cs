using DSharpPlus.Entities;

namespace masz.Models
{
    public class UserNoteView
    {
        public UserNote UserNote { get; set; }
        public DiscordUser User { get; set; }
        public DiscordUser Moderator { get; set; }
    }
}
