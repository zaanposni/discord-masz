using Discord;
using MASZ.Models.Database;

namespace MASZ.Models.Views
{
    public class VerifiedEvidenceExpandedView
    {
        public VerifiedEvidenceView Evidence { get; set; }
        public DiscordUserView Reporter { get; set; }
        public DiscordUserView Reported { get; set; }
        public List<CaseView> LinkedCases { get; set; }

        public VerifiedEvidenceExpandedView(VerifiedEvidence evidence, IUser reporter, IUser reported)
        {
            Evidence = new VerifiedEvidenceView(evidence);
            Reporter = DiscordUserView.CreateOrDefault(reporter);
            Reported = DiscordUserView.CreateOrDefault(reported);
            LinkedCases = new List<CaseView>();
        }
    }
}
