using MASZ.Models;

namespace MASZ.Events
{
    public class ModCaseDeletedEventArgs : EventArgs
    {
        private readonly ModCase _modCase;

        public ModCaseDeletedEventArgs(ModCase modCase)
        {
            _modCase = modCase;
        }

        public ModCase GetModCase()
        {
            return _modCase;
        }
    }
}