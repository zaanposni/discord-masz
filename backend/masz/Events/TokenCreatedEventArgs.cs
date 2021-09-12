using System;
using masz.Models;

namespace masz.Events
{
    public class TokenCreatedEventArgs : EventArgs
    {
        private APIToken _token;

        public TokenCreatedEventArgs(APIToken token)
        {
            _token = token;
        }

        public APIToken GetToken()
        {
            return _token;
        }
    }
}