using System;
using masz.Enums;

namespace masz.Models
{
    public interface QuickSearchEntry
    {
        DateTime CreatedAt { get; set; }
    }
    public class QuickSearchEntry<T> : QuickSearchEntry
    {
        public T Entry { get; set; }
        public DateTime CreatedAt { get; set; }
        public QuickSearchEntryType QuickSearchEntryType { get; set; }
    }
}
