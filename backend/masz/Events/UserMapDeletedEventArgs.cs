using MASZ.Models;

namespace MASZ.Events
{
    public class UserMapDeletedEventArgs : EventArgs
    {
        private readonly UserMapping _userMapping;

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