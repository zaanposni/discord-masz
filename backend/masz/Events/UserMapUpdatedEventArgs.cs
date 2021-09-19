using System;
using masz.Models;

namespace masz.Events
{
    public class UserMapUpdatedEventArgs : EventArgs
    {
        private UserMapping _userMapping;

        public UserMapUpdatedEventArgs(UserMapping userMapping)
        {
            _userMapping = userMapping;
        }

        public UserMapping GetUserMapping()
        {
            return _userMapping;
        }
    }
}