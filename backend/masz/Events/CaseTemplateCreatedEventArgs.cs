using MASZ.Models;

namespace MASZ.Events
{
    public class CaseTemplateCreatedEventArgs : EventArgs
    {
        private readonly CaseTemplate _template;

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