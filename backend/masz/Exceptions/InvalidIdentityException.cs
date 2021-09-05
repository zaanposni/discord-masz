namespace masz.Exceptions
{
    public class InvalidIdentityException : BaseAPIException
    {
        public string Token { get; set; }
        public InvalidIdentityException(string message, string token) : base(message)
        {
            Token = token;
        }
        public InvalidIdentityException(string token) : base("Invalid identity (token) encountered.")
        {
            Token = token;
        }
        public InvalidIdentityException() : base("Invalid identity (token) encountered.")
        {
        }
    }
}