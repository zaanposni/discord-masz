using DSharpPlus.Entities;

namespace masz.Models
{
    public class ModCaseTableEntry
    {
        public ModCase ModCase { get; set; }
        public DiscordUser Moderator { get; set; }
        public DiscordUser Suspect { get; set; }

        public void RemoveModeratorInfo()
        {
            this.Moderator = null;
            this.ModCase.RemoveModeratorInfo();
        }
    }
}
