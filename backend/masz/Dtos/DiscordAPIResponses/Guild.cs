using Newtonsoft.Json;

namespace masz.Dtos.DiscordAPIResponses
{
    public class Guild
    {
        public Guild() { }
        public Guild(string json)
        {
            var temp = JsonConvert.DeserializeObject<Guild>(json);
            Id = temp.Id;
            Name = temp.Name;
            Icon = temp.Icon;
        }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("icon")]
        public string Icon { get; set; }
    }
}