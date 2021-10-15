using System;
using masz.Models;

namespace masz.Events
{
    public class ModCaseDeletedEventArgs : EventArgs
    {
        private ModCase _modCase;

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