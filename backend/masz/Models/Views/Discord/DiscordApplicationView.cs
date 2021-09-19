using DSharpPlus.Entities;

namespace masz.Models.Views
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

        private DiscordApplicationView() { }
        private DiscordApplicationView(DiscordApplication application)
        {
            if (application == null) return;
            Id = application.Id.ToString();
            Name = application.Name;
            Description = application.Description;
            IconUrl = application.Icon;
            IconHash = application.IconHash;
            PrivacyPolicyUrl = application.PrivacyPolicyUrl;
            TermsOfServiceUrl = application.TermsOfServiceUrl;
        }

        public static DiscordApplicationView CreateOrDefault(DiscordApplication application)
        {
            if (application == null) return null;
            if (application.Id == 0) return null;
            return new DiscordApplicationView(application);
        }
    }
}
