using DSharpPlus.Entities;

namespace masz.Models
{
    public class UserInviteView
    {
        public UserInviteView(UserInvite userInvite, DiscordUser invitedUser, DiscordUser invitedBy)
        {
            this.UserInvite = userInvite;
            this.InvitedUser = invitedUser;
            this.InvitedBy = invitedBy;
        }
        public UserInvite UserInvite { get; set; }
        public DiscordUser InvitedUser { get; set; }
        public DiscordUser InvitedBy { get; set; }
    }
}