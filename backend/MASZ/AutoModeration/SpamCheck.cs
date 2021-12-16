using Discord;
using MASZ.Models;

namespace MASZ.AutoModeration
{
    public static class SpamCheck
    {
        // { guildid -> { userid -> [ timestamps ] } }
        private static readonly Dictionary<ulong, Dictionary<ulong, List<long>>> msgBoard = new();
        public static bool Check(IMessage message, AutoModerationConfig config, IDiscordClient _)
        {
            if (config.Limit == null)
            {
                return false;
            }
            if (config.TimeLimitMinutes == null)
            {
                return false;
            }
            if (message.Embeds == null)
            {
                return false;
            }

            // set guild config in msg_board if it doesn't exist
            if (!msgBoard.ContainsKey((message.Channel as ITextChannel).Guild.Id))
            {
                msgBoard[(message.Channel as ITextChannel).Guild.Id] = new Dictionary<ulong, List<long>>();
            }

            long timestamp = message.CreatedAt.ToUnixTimeSeconds();

            // filter out messages older than TimeLimitMinutes
            // delta is the time minus the TimeLimitMinutes => the time messages older than should be deleted
            // not using *60 because we are working with seconds here
            long delta = timestamp - (long)config.TimeLimitMinutes;

            foreach (ulong userId in msgBoard[(message.Channel as ITextChannel).Guild.Id].Keys.ToList())
            {
                var newTimestamps = msgBoard[(message.Channel as ITextChannel).Guild.Id][userId].FindAll(x => x > delta);
                if (newTimestamps.Count > 0)
                {
                    msgBoard[(message.Channel as ITextChannel).Guild.Id][userId] = newTimestamps;
                }
                else
                {
                    msgBoard[(message.Channel as ITextChannel).Guild.Id].Remove(userId);
                }
            }

            // add the message to the "msg_board"
            if (!msgBoard[(message.Channel as ITextChannel).Guild.Id].ContainsKey(message.Author.Id))
            {
                msgBoard[(message.Channel as ITextChannel).Guild.Id][message.Author.Id] = new List<long>() { timestamp };
            }
            else
            {
                msgBoard[(message.Channel as ITextChannel).Guild.Id][message.Author.Id].Add(timestamp);
            }

            // count the number of messages and check them for being too high
            return msgBoard[(message.Channel as ITextChannel).Guild.Id][message.Author.Id].Count > config.Limit;
        }
    }
}