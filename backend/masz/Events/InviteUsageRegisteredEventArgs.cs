using System;
using masz.Models;

namespace masz.Events
{
    public class InviteUsageRegisteredEventArgs : EventArgs
    {
        private UserInvite _userInivte;

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