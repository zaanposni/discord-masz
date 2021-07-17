using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using masz.Dtos.DiscordAPIResponses;

namespace masz.Models
{
    public class ModCaseTableEntry
    {
        public ModCase ModCase { get; set; }
        public User Moderator { get; set; }
        public User Suspect { get; set; }

        public void RemoveModeratorInfo()
        {
            this.Moderator = null;
            this.ModCase.RemoveModeratorInfo();
        }
    }
}
