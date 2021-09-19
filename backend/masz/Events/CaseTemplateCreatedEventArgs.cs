using System;
using masz.Models;

namespace masz.Events
{
    public class CaseTemplateCreatedEventArgs : EventArgs
    {
        private CaseTemplate _template;

        public CaseTemplateCreatedEventArgs(CaseTemplate template)
        {
            _template = template;
        }

        public CaseTemplate GetTemplate()
        {
            return _template;
        }
    }
}