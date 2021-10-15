using DSharpPlus.Entities;
using masz.Models.Views;

namespace masz.Models
{
    public class ModCaseTableEntry
    {
        public CaseView ModCase { get; set; }
        public DiscordUserView Moderator { get; set; }
        public DiscordUserView Suspect { get; set; }

        public void RemoveModeratorInfo()
        {
            this.Moderator = null;
            this.ModCase.RemoveModeratorInfo();
        }

        public ModCaseTableEntry(ModCase modCase, DiscordUser moderator, DiscordUser suspect)
        {
            ModCase = new CaseView(modCase);
            Moderator = DiscordUserView.CreateOrDefault(moderator);
            Suspect = DiscordUserView.CreateOrDefault(suspect);
        }
    }
}
