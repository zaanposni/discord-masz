using System.Collections.Generic;
using Newtonsoft.Json;

namespace masz.Dtos.DiscordAPIResponses
{
    public class Ban
    {
        public Ban() { }
        public Ban(string json)
        {
            var temp = JsonConvert.DeserializeObject<Ban>(json);
            Reason = temp.Reason;
            User = new User(JsonConvert.SerializeObject(temp.User));
        }
        [JsonProperty("reason")]
        public string Reason { get; set; }
        [JsonProperty("user")]
        public User User { get; set; }
    }
}