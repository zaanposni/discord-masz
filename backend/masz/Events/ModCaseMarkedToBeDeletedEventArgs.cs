using MASZ.Models;

namespace MASZ.Events
{
    public class ModCaseMarkedToBeDeletedEventArgs : EventArgs
    {
        private readonly ModCase _modCase;

        public ModCaseMarkedToBeDeletedEventArgs(ModCase modCase)
        {
            _modCase = modCase;
        }

        public ModCase GetModCase()
        {
            return _modCase;
        }
    }
}