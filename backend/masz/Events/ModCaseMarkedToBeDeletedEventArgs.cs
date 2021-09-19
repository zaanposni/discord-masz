using System;
using masz.Models;

namespace masz.Events
{
    public class ModCaseMarkedToBeDeletedEventArgs : EventArgs
    {
        private ModCase _modCase;

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