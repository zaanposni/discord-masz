using MASZ.Models;

namespace MASZ.Events
{
    public class ModCaseRestoredEventArgs : EventArgs
    {
        private readonly ModCase _modCase;

        public ModCaseRestoredEventArgs(ModCase modCase)
        {
            _modCase = modCase;
        }

        public ModCase GetModCase()
        {
            return _modCase;
        }
    }
}