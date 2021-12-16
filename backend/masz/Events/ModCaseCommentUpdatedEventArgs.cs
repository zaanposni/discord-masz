using MASZ.Models;

namespace MASZ.Events
{
    public class ModCaseCommentUpdatedEventArgs : EventArgs
    {
        private readonly ModCaseComment _modCaseComment;

        public ModCaseCommentUpdatedEventArgs(ModCaseComment modCaseComment)
        {
            _modCaseComment = modCaseComment;
        }

        public ModCaseComment GetModCaseComment()
        {
            return _modCaseComment;
        }
    }
}