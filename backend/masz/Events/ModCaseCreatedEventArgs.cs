using MASZ.Models;

namespace MASZ.Events
{
    public class ModCaseCreatedEventArgs : EventArgs
    {
        private readonly ModCase _modCase;

        public ModCaseCreatedEventArgs(ModCase modCase)
        {
            _modCase = modCase;
        }

        public ModCase GetModCase()
        {
            return _modCase;
        }
    }
}