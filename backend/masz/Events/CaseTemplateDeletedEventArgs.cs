using MASZ.Models;

namespace MASZ.Events
{
    public class CaseTemplateDeletedEventArgs : EventArgs
    {
        private readonly CaseTemplate _template;

        public CaseTemplateDeletedEventArgs(CaseTemplate template)
        {
            _template = template;
        }

        public CaseTemplate GetTemplate()
        {
            return _template;
        }
    }
}