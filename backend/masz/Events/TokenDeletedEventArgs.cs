using MASZ.Models;

namespace MASZ.Events
{
    public class TokenDeletedEventArgs : EventArgs
    {
        private readonly APIToken _token;

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