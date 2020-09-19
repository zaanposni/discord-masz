using System;
using Newtonsoft.Json;

namespace masz.Dtos.DiscordAPIResponses
{
    public class Message
    {
        public string Id { get; set; }
        [JsonProperty("channel_id")]
        public string ChannelId { get; set; }
        [JsonProperty("guild_id")]
        public string GuildId { get; set; }
        public User Author { get; set; }
        public GuildMember Member { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        [JsonProperty("edited_timestamp")]
        public DateTime EditedTimestamp { get; set; }
    }
}