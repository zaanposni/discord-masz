using System;
using masz.Models;

namespace masz.Events
{
    public class TokenDeletedEventArgs : EventArgs
    {
        private APIToken _token;

        public TokenDeletedEventArgs(APIToken token)
        {
            _token = token;
        }

        public APIToken GetToken()
        {
            return _token;
        }
    }
}