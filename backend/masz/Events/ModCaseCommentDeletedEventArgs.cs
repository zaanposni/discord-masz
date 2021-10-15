using System;
using masz.Models;

namespace masz.Events
{
    public class ModCaseCommentDeletedEventArgs : EventArgs
    {
        private ModCaseComment _modCaseComment;

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