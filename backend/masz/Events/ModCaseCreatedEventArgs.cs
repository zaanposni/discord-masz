using System;
using masz.Models;

namespace masz.Events
{
    public class ModCaseCreatedEventArgs : EventArgs
    {
        private ModCase _modCase;

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