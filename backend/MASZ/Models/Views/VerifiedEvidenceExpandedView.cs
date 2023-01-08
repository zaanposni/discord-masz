using Discord;
using MASZ.Models.Database;

namespace MASZ.Models.Views
{
    public class VerifiedEvidenceExpandedView
    {
        public VerifiedEvidenceView Evidence { get; set; }
        public DiscordUserView Reported { get; set; }
        public DiscordUserView Moderator { get; set; }
        public List<CaseView> LinkedCases { get; set; }

        public VerifiedEvidenceExpandedView(VerifiedEvidence evidence, IUser reported, IUser moderator)
        {
            Evidence = new VerifiedEvidenceView(evidence);
            Reported = DiscordUserView.CreateOrDefault(reported);
            Moderator = DiscordUserView.CreateOrDefault(moderator);
            LinkedCases = new List<CaseView>();
        }

        public void RemoveModeratorInfo()
        {
            Evidence.RemoveModeratorInfo();
            Moderator = null;
        }
    }
}
