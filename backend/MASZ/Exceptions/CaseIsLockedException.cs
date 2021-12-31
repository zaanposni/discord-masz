using MASZ.Enums;

namespace MASZ.Exceptions
{
    public class CaseIsLockedException : BaseAPIException
    {
        public CaseIsLockedException(string message) : base(message, APIError.ModCaseDoesNotAllowComments)
        {
        }
        public CaseIsLockedException() : base("Case is locked.", APIError.ModCaseDoesNotAllowComments)
        {
        }
    }
}