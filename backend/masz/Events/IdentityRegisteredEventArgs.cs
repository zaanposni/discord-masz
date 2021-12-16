using MASZ.Models;

namespace MASZ.Events
{
    public class IdentityRegisteredEventArgs : EventArgs
    {
        private readonly Identity _identity;

        public IdentityRegisteredEventArgs(Identity identity)
        {
            _identity = identity;
        }

        public Identity GetIdentity()
        {
            return _identity;
        }

        public bool IsTokenIdentity()
        {
            return _identity is TokenIdentity;
        }

        public bool IsOAuthIdentity()
        {
            return _identity is DiscordOAuthIdentity;
        }

        public bool IsCommandIdentity()
        {
            return _identity is DiscordCommandIdentity;
        }
    }
}