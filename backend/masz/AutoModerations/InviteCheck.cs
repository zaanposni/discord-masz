using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using masz.Models;

namespace masz.AutoModerations
{
    public static class InviteChecker
    {
        private static readonly Regex _inviteRegex = new Regex(@"(https?:\/\/)?(www\.)?(discord(app)?\.(gg|io|me|li|com)(\/invite)?)\/(?![a-z]+\/)([^\s]+)");
        public static async Task<bool> Check(DiscordMessage message, AutoModerationConfig config, DiscordClient client)
        {
            if (string.IsNullOrEmpty(message.Content))
            {
                return false;
            }

            List<string> ignoreGuilds = new List<string>();
            if (!string.IsNullOrEmpty(config.CustomWordFilter))
            {
                ignoreGuilds = config.CustomWordFilter.Split('\n').ToList();
            }

            var matches = _inviteRegex.Matches(message.Content);

            if (matches.Count != 0)
            {
                List<string> alreadyChecked = new List<string>();
                foreach (Match usedInvite in matches)
                {
                    try
                    {
                        string inviteCode = usedInvite.Groups.Values.Last().ToString().Trim();
                        if (alreadyChecked.Contains(inviteCode))
                        {
                            continue;
                        }
                        alreadyChecked.Append(inviteCode);
                        DiscordInvite fetchedInvite = await client.GetInviteByCodeAsync(inviteCode);
                        if (fetchedInvite.Guild.Id != message.Channel.GuildId && !ignoreGuilds.Contains(fetchedInvite.Guild.Id.ToString()))
                        {
                            return true;
                        }
                    } catch (Exception e)
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