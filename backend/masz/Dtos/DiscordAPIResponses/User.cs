using System;
using Newtonsoft.Json;

namespace masz.Dtos.DiscordAPIResponses
{
    public class User
    {
        public User() { }
        public User(string json)
        {
            var temp = JsonConvert.DeserializeObject<User>(json);
            Id = temp.Id;
            Username = temp.Username;
            Discriminator = temp.Discriminator;
            Avatar = temp.Avatar;
            Bot = temp.Bot;
        }

        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("discriminator")]
        public string Discriminator { get; set; }
        [JsonProperty("avatar")]
        public string Avatar { get; set; }
        [JsonProperty("bot")]
        public bool Bot { get; set; }
    }
}