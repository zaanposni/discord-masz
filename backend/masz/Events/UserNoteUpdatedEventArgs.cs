using MASZ.Models;

namespace MASZ.Events
{
    public class UserNoteUpdatedEventArgs : EventArgs
    {
        private readonly UserNote _userNote;

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