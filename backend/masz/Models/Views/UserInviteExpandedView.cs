using DSharpPlus.Entities;
using masz.Models.Views;

namespace masz.Models
{
    public class UserInviteExpandedView
    {
        public UserInviteExpandedView(UserInvite userInvite, DiscordUser invitedUser, DiscordUser invitedBy)
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