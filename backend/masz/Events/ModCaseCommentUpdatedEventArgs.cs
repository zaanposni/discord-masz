using System;
using masz.Models;

namespace masz.Events
{
    public class ModCaseCommentUpdatedEventArgs : EventArgs
    {
        private ModCaseComment _modCaseComment;

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