using System.Text.RegularExpressions;
using DSharpPlus.Entities;
using masz.Models;

namespace masz.AutoModerations
{
    public static class InviteChecker
    {
        private static readonly Regex _inviteRegex = new Regex(@"(https?:\/\/)?(www\.)?(discord(app)?\.(gg|io|me|li|com)(\/invite)?)\/(?!.+\/.+).+[a-zA-Z0-9]");
        public static bool Check(DiscordMessage message, AutoModerationConfig config)
        {
            if (string.IsNullOrEmpty(message.Content))
            {
                return false;
            }
            return _inviteRegex.Match(message.Content).Success;
        }
    }
}