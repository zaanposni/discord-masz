using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using masz.Dtos.DiscordAPIResponses;

namespace masz.Models
{
    public class AutoModerationEventTableEntry
    {
        public AutoModerationEvent AutoModerationEvent { get; set; }
        public User Suspect { get; set; }
    }
}
