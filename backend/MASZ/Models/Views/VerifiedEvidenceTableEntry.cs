using Discord;
using MASZ.Models.Database;

namespace MASZ.Models.Views
{
    public class VerifiedEvidenceTableEntry
    {
        public VerifiedEvidenceView VerifiedEvidence { get; set; }
        public DiscordUserView Reporter { get; set; }
        public DiscordUserView Reported { get; set; }

        public VerifiedEvidenceTableEntry(VerifiedEvidence verifiedEvidence, IUser reporter, IUser reported)
        {
            VerifiedEvidence = new VerifiedEvidenceView(verifiedEvidence);
            Reporter = DiscordUserView.CreateOrDefault(reporter);
            Reported = DiscordUserView.CreateOrDefault(reported);
        }
    }
}
