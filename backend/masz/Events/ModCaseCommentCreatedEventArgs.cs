using System;
using masz.Models;

namespace masz.Events
{
    public class ModCaseCommentCreatedEventArgs : EventArgs
    {
        private ModCaseComment _modCaseComment;

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