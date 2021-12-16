using MASZ.Models;

namespace MASZ.Events
{
    public class UserNoteDeletedEventArgs : EventArgs
    {
        private readonly UserNote _userNote;

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