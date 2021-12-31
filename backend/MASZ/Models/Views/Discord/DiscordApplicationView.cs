using Discord;
using MASZ.Extensions;

namespace MASZ.Models.Views
{
    public class DiscordApplicationView
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string IconUrl { get; set; }
        public string IconHash { get; set; }
        public string PrivacyPolicyUrl { get; set; }
        public string TermsOfServiceUrl { get; set; }

        private DiscordApplicationView(IApplication application)
        {
            if (application == null) return;
            Id = application.Id.ToString();
            Name = application.Name;
            Description = application.Description;
            IconUrl = application.IconUrl.GetAnimatedOrDefaultAvatar();
            IconHash = application.IconUrl?.GetAnimatedOrDefaultAvatar().Split('/').Last();
            PrivacyPolicyUrl = application?.PrivacyPolicy ?? "";
            TermsOfServiceUrl = application?.TermsOfService ?? "";
        }

        public static DiscordApplicationView CreateOrDefault(IApplication application)
        {
            if (application == null) return null;
            if (application.Id == 0) return null;
            return new DiscordApplicationView(application);
        }
    }
}
