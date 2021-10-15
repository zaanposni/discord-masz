using System;
using masz.Models;

namespace masz.Events
{
    public class ModCaseUpdatedEventArgs : EventArgs
    {
        private ModCase _modCase;

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