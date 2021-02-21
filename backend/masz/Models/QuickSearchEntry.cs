using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using masz.Dtos.DiscordAPIResponses;

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

    public enum QuickSearchEntryType
    {
        ModCase,
        AutoModeration
    }
}
