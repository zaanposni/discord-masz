using System.Collections.Generic;
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
            Roles = new List<Role>();
            foreach (var role in temp.Roles)
            {
                Roles.Add(new Role(JsonConvert.SerializeObject(role)));
            }
        }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("icon")]
        public string Icon { get; set; }
        [JsonProperty("roles")]
        public List<Role> Roles { get; set; }
    }
}