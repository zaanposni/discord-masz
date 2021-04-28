
using masz.Dtos.DiscordAPIResponses;

namespace masz.Models
{
    public class UserInviteView
    {
        public UserInviteView(UserInvite userInvite, User invitedUser, User invitedBy)
        {
            this.UserInvite = userInvite;
            this.InvitedUser = invitedUser;
            this.InvitedBy = invitedBy;
        }
        public UserInvite UserInvite { get; set; }
        public User InvitedUser { get; set; }
        public User InvitedBy { get; set; }
    }
}