using MASZ.Enums;
using MASZ.Models;

namespace MASZ.Exceptions
{
    public class ProtectedModCaseSuspectException : BaseAPIException
    {
        public ModCase ModCase { get; set; }
        public ProtectedModCaseSuspectException(string message, ModCase modCase) : base(message, APIError.ProtectedModCaseSuspect)
        {
            ModCase = modCase;
        }
        public ProtectedModCaseSuspectException(ModCase modCase) : base("User is protected from modcases.", APIError.ProtectedModCaseSuspect)
        {
            ModCase = modCase;
        }
        public ProtectedModCaseSuspectException() : base("User is protected from modcases.", APIError.ProtectedModCaseSuspect)
        {
        }
    }
}