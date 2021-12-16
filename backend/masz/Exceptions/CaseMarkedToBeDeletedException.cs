using MASZ.Enums;

namespace MASZ.Exceptions
{
    public class CaseMarkedToBeDeletedException : BaseAPIException
    {
        public CaseMarkedToBeDeletedException(string message) : base(message, APIError.ModCaseIsMarkedToBeDeleted)
        {
        }
        public CaseMarkedToBeDeletedException() : base("Case is marked to be deleted.", APIError.ModCaseIsMarkedToBeDeleted)
        {
        }
    }
}