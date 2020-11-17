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

            if (Avatar != null) {
                if (Avatar.StartsWith("a_")) { // gif avatar
                    ImageUrl = $"https://cdn.discordapp.com/avatars/{Id}/{Avatar}.gif";
                } else {
                    ImageUrl = $"https://cdn.discordapp.com/avatars/{Id}/{Avatar}.png";
                }
            } else {
                string defaultAvatar = (Int32.Parse(Discriminator) % 5).ToString();
                ImageUrl = $"https://cdn.discordapp.com/embed/avatars/{defaultAvatar}.png";
            }
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
        public string ImageUrl { get; set; }
    }
}