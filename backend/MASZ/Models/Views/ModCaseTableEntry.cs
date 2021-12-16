using Discord;
using MASZ.Models.Views;

namespace MASZ.Models
{
    public class ModCaseTableEntry
    {
        public CaseView ModCase { get; set; }
        public DiscordUserView Moderator { get; set; }
        public DiscordUserView Suspect { get; set; }

        public void RemoveModeratorInfo()
        {
            Moderator = null;
            ModCase.RemoveModeratorInfo();
        }

        public ModCaseTableEntry(ModCase modCase, IUser moderator, IUser suspect)
        {
            ModCase = new CaseView(modCase);
            Moderator = DiscordUserView.CreateOrDefault(moderator);
            Suspect = DiscordUserView.CreateOrDefault(suspect);
        }
    }
}
