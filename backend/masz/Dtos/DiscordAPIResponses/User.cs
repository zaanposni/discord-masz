using System;

namespace masz.Dtos.DiscordAPIResponses
{
    public class User
    {
        public User() {  }
        public string id { get; set; }
        public string username { get; set; }
        public string discriminator { get; set; }
        public string avatar { get; set; }
    }
}