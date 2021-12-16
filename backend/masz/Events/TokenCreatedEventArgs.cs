using MASZ.Models;

namespace MASZ.Events
{
    public class TokenCreatedEventArgs : EventArgs
    {
        private readonly APIToken _token;

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