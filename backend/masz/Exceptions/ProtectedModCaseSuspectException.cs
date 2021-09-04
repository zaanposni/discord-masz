using masz.Models;

namespace masz.Exceptions
{
    public class ProtectedModCaseSuspectException : BaseAPIException
    {
        public ModCase ModCase { get; set; }
        public ProtectedModCaseSuspectException(string message, ModCase modCase) : base(message)
        {
            ModCase = modCase;
        }
        public ProtectedModCaseSuspectException(ModCase modCase) : base("User is protected from modcases.")
        {
            ModCase = modCase;
        }
        public ProtectedModCaseSuspectException() : base("User is protected from modcases.")
        {
        }
    }
}