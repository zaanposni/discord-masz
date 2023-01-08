using Discord;
using MASZ.Models.Database;

namespace MASZ.Models.Views
{
    public class VerifiedEvidenceTableEntry
    {
        public VerifiedEvidenceView VerifiedEvidence { get; set; }
        public DiscordUserView Reported { get; set; }
        public DiscordUserView Moderator { get; set; }

        public VerifiedEvidenceTableEntry(VerifiedEvidence verifiedEvidence, IUser reported, IUser moderator)
        {
            VerifiedEvidence = new VerifiedEvidenceView(verifiedEvidence);
            Reported = DiscordUserView.CreateOrDefault(reported);
            Moderator = DiscordUserView.CreateOrDefault(moderator);
        }

        public void RemoveModeratorInfo()
        {
            VerifiedEvidence.RemoveModeratorInfo();
            Moderator = null;
        }
    }
}
