using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace masz.Dtos.DiscordAPIResponses
{
    public class GuildMember
    {
        public GuildMember() { }

        public GuildMember(string json)
        {
            var temp = JsonConvert.DeserializeObject<GuildMember>(json);
            User = new User(JsonConvert.SerializeObject(temp.User));
            Nick = temp.Nick;
            Roles = temp.Roles;
            JoinedAt = temp.JoinedAt;
        }

        [JsonProperty("user")]
        public User User { get; set; }
        [JsonProperty("nick")]
        public string Nick { get; set; }
        [JsonProperty("roles")]
        public List<string> Roles { get; set; }
        [JsonProperty("joined_at")]
        public DateTime JoinedAt { get; set; }
    }
}
