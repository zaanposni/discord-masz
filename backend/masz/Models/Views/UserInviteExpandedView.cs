using Discord;
using MASZ.Models.Views;

namespace MASZ.Models
{
    public class UserInviteExpandedView
    {
        public UserInviteExpandedView(UserInvite userInvite, IUser invitedUser, IUser invitedBy)
        {
            UserInvite = new UserInviteView(userInvite);
            InvitedUser = DiscordUserView.CreateOrDefault(invitedUser);
            InvitedBy = DiscordUserView.CreateOrDefault(invitedBy);
        }
        public UserInviteView UserInvite { get; set; }
        public DiscordUserView InvitedUser { get; set; }
        public DiscordUserView InvitedBy { get; set; }
    }
}