using System;
using masz.Models;

namespace masz.Events
{
    public class UserMapDeletedEventArgs : EventArgs
    {
        private UserMapping _userMapping;

        public UserMapDeletedEventArgs(UserMapping userMapping)
        {
            _userMapping = userMapping;
        }

        public UserMapping GetUserMapping()
        {
            return _userMapping;
        }
    }
}