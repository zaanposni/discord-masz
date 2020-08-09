using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace masz.Dtos.DiscordAPIResponses
{
    public class UserGuilds
    {
        public string id { get; set; }
        public string name { get; set; }
        public bool owner { get; set; }
        public string owner_id { get; set; }
    }
}
