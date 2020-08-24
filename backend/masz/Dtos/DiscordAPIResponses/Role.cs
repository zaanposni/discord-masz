using Newtonsoft.Json;

namespace masz.Dtos.DiscordAPIResponses
{
    public class Role
    {
        public Role() { }
        public Role(string json)
        {
            var temp = JsonConvert.DeserializeObject<Role>(json);
            Id = temp.Id;
            Name = temp.Name;
            Color = temp.Color;
        }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("color")]
        public int Color { get; set; }
    }
}