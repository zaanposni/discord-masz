using System;
using masz.Models;

namespace masz.Events
{
    public class UserNoteUpdatedEventArgs : EventArgs
    {
        private UserNote _userNote;

        public UserNoteUpdatedEventArgs(UserNote userNote)
        {
            _userNote = userNote;
        }

        public UserNote GetUserNote()
        {
            return _userNote;
        }
    }
}