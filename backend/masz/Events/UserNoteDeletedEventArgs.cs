using System;
using masz.Models;

namespace masz.Events
{
    public class UserNoteDeletedEventArgs : EventArgs
    {
        private UserNote _userNote;

        public UserNoteDeletedEventArgs(UserNote userNote)
        {
            _userNote = userNote;
        }

        public UserNote GetUserNote()
        {
            return _userNote;
        }
    }
}