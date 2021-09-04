using DSharpPlus.Entities;

namespace masz.Models
{
    public class TemplateView
    {
        public CaseTemplate CaseTemplate { get; set; }
        public DiscordUser Creator { get; set; }
        public DiscordGuild Guild { get; set; }
    }
}
