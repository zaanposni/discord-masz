using DSharpPlus.Entities;

namespace masz.Models.Views
{
    public class CaseTemplateExpandedView
    {
        public CaseTemplateView CaseTemplate { get; set; }
        public DiscordUserView Creator { get; set; }
        public DiscordGuildView Guild { get; set; }

        public CaseTemplateExpandedView(CaseTemplate template, DiscordUser creator, DiscordGuild guild)
        {
            CaseTemplate = new CaseTemplateView(template);
            Creator = DiscordUserView.CreateOrDefault(creator);
            Guild = new DiscordGuildView(guild);
        }
    }
}
