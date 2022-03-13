using Discord;
using MASZ.Models;
using System.Text.RegularExpressions;

namespace MASZ.AutoModeration
{
    public static class InviteChecker
    {
        private static readonly Regex _inviteRegex = new(@"(https?:\/\/)?(www\.)?(discord(app)?\.(gg|io|me|li|com)(\/invite)?)\/(?![a-z]+\/)([^\?\s]+)(\?event=([^\s]+))?");
        public static async Task<bool> Check(IMessage message, AutoModerationConfig config, IDiscordClient client)
        {
            if (string.IsNullOrEmpty(message.Content))
            {
                return false;
            }

            List<string> ignoreGuilds = new();
            if (!string.IsNullOrEmpty(config.CustomWordFilter))
            {
                ignoreGuilds = config.CustomWordFilter.Split('\n').ToList();
            }

            var matches = _inviteRegex.Matches(message.Content);

            if (matches.Count != 0)
            {
                List<string> alreadyChecked = new();
                foreach (Match usedInvite in matches)
                {
                    try
                    {
                        string inviteCode = usedInvite.Groups.Values.Skip(7).First().ToString().Trim();
                        if (alreadyChecked.Contains(inviteCode))
                        {
                            continue;
                        }
                        alreadyChecked.Add(inviteCode);
                        IInvite fetchedInvite = await client.GetInviteAsync(inviteCode);
                        if (fetchedInvite.GuildId != (message.Channel as ITextChannel).GuildId && !ignoreGuilds.Contains(fetchedInvite.GuildId.ToString()))
                        {
                            return true;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        return true;
                    }
                }
                return false;
            }
            return false;
        }
    }
}