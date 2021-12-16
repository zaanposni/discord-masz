using MASZ.Models;

namespace MASZ.Events
{
    public class UserMapUpdatedEventArgs : EventArgs
    {
        private readonly UserMapping _userMapping;

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