using MASZ.Models;

namespace MASZ.Events
{
    public class ModCaseUpdatedEventArgs : EventArgs
    {
        private readonly ModCase _modCase;

        public ModCaseUpdatedEventArgs(ModCase modCase)
        {
            _modCase = modCase;
        }

        public ModCase GetModCase()
        {
            return _modCase;
        }
    }
}