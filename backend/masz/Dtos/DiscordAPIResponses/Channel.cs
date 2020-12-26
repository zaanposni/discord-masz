using System.Collections.Generic;
using Newtonsoft.Json;

namespace masz.Dtos.DiscordAPIResponses
{
    public class Channel
    {
        public Channel() { }
        public Channel(string json)
        {
            var temp = JsonConvert.DeserializeObject<Channel>(json);
            Id = temp.Id;
            Name = temp.Name;
            Type = temp.Type;
        }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
    }
}