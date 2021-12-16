using MASZ.Models;

namespace MASZ.Events
{
    public class ModCaseCommentDeletedEventArgs : EventArgs
    {
        private readonly ModCaseComment _modCaseComment;

        public ModCaseCommentDeletedEventArgs(ModCaseComment modCaseComment)
        {
            _modCaseComment = modCaseComment;
        }

        public ModCaseComment GetModCaseComment()
        {
            return _modCaseComment;
        }
    }
}