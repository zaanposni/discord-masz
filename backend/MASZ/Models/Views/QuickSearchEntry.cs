using MASZ.Enums;

namespace MASZ.Models
{
    public interface IQuickSearchEntry
    {
        DateTime CreatedAt { get; set; }
    }

    public class QuickSearchEntry<T> : IQuickSearchEntry
    {
        public T Entry { get; set; }
        public DateTime CreatedAt { get; set; }
        public QuickSearchEntryType QuickSearchEntryType { get; set; }
    }
}
