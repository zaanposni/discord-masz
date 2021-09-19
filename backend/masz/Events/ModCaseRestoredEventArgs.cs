using System;
using masz.Models;

namespace masz.Events
{
    public class ModCaseRestoredEventArgs : EventArgs
    {
        private ModCase _modCase;

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