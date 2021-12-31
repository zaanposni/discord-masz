using MASZ.Enums;

namespace MASZ.Exceptions
{
    public class CaseNotMarkedToBeDeletedException : BaseAPIException
    {
        public CaseNotMarkedToBeDeletedException(string message) : base(message, APIError.ModCaseIsNotMarkedToBeDeleted)
        {
        }
        public CaseNotMarkedToBeDeletedException() : base("Case is marked to be deleted.", APIError.ModCaseIsNotMarkedToBeDeleted)
        {
        }
    }
}