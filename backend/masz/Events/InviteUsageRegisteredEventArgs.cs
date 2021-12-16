using MASZ.Models;

namespace MASZ.Events
{
    public class InviteUsageRegisteredEventArgs : EventArgs
    {
        private readonly UserInvite _userInivte;

        public InviteUsageRegisteredEventArgs(UserInvite userInvite)
        {
            _userInivte = userInvite;
        }

        public UserInvite GetUserInvite()
        {
            return _userInivte;
        }
    }
}