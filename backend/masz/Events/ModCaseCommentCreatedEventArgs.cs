using MASZ.Models;

namespace MASZ.Events
{
    public class ModCaseCommentCreatedEventArgs : EventArgs
    {
        private readonly ModCaseComment _modCaseComment;

        public ModCaseCommentCreatedEventArgs(ModCaseComment modCaseComment)
        {
            _modCaseComment = modCaseComment;
        }

        public ModCaseComment GetModCaseComment()
        {
            return _modCaseComment;
        }
    }
}